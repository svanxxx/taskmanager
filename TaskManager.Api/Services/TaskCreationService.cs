using System.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Infrastructure;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

/// <summary>
/// Creates a task the way TRService.newTask4MeNow does:
/// insert DEFECTS from field defaults, insert REPORTBY, and set the assignee,
/// estimate, working disposition, schedule order, and modifier.
/// </summary>
public sealed class TaskCreationService
{
    const int DefaultEntityType = 1684431732;
    const int SummaryMaxLength = 255;
    const int ProjectId = 1;
    const decimal DefaultConfigId = 4294967293m;

    static readonly string[] DefaultFieldCodes = ["PROD", "TYPE", "DISP", "PRIO", "COMP", "SEVR"];

    static readonly string[] EmailClaimTypes =
    [
        ClaimTypes.Email,
        "email",
        "preferred_username",
        "upn",
        ClaimTypes.Upn,
        "unique_name"
    ];

    readonly TaskDbContext _db;
    readonly PersonsDbContext _persons;
    readonly ILogger<TaskCreationService> _logger;

    public TaskCreationService(TaskDbContext db, PersonsDbContext persons, ILogger<TaskCreationService> logger)
    {
        _db = db;
        _persons = persons;
        _logger = logger;
    }

    public async Task<int> CreateAsync(string? summary, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(summary))
        {
            throw new RequestValidationException("Summary is required.");
        }

        if (summary.Length > SummaryMaxLength)
        {
            throw new RequestValidationException("Summary cannot exceed 255 characters.");
        }

        var actor = await ResolveActorAsync(principal, cancellationToken);
        var defaults = await LoadDefaultsAsync(cancellationToken);
        var workingDisposition = await LoadWorkingDispositionAsync(cancellationToken);

        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var now = DateTime.UtcNow;
        var nextRecord = (await _db.Defects.MaxAsync(d => (decimal?)d.IdRecord, cancellationToken) ?? 0) + 1;
        var nextNumber = (await _db.Defects.MaxAsync(d => (decimal?)d.DefectNum, cancellationToken) ?? 0) + 1;
        if (nextNumber is < 1 or > int.MaxValue)
        {
            throw new InvalidOperationException("The database returned a task id outside the legacy 32-bit range.");
        }

        var defect = new Defect
        {
            ProjectId = ProjectId,
            IdRecord = nextRecord,
            DateCreate = now,
            CreatedBy = actor.TtUserId,
            DateModify = now,
            ModifiedBy = actor.TtUserId,
            DefectNum = nextNumber,
            Summary = summary,
            Status = 1,
            InitialStatus = 1,
            TypeId = defaults.Type,
            ProductId = defaults.Product,
            EnteredBy = actor.TtUserId,
            DispositionId = workingDisposition,
            PriorityId = defaults.Priority,
            ComponentId = defaults.Component,
            SeverityId = defaults.Severity,
            DateEnter = now,
            AddLocation = 1,
            AssignedUserId = actor.TtUserId,
            Estimate = 1,
            Order = 1,
            Modifier = actor.Email
        };
        _db.Defects.Add(defect);

        var nextReport = (await _db.Reports.MaxAsync(r => (decimal?)r.IdRecord, cancellationToken) ?? 0) + 1;
        _db.Reports.Add(new ReportBy
        {
            IdRecord = nextReport,
            ProjectId = ProjectId,
            FoundBy = actor.TtUserId,
            DateFound = now,
            OrderNum = 1,
            ReproducedId = 0,
            TestConditionType = 1,
            ConfigId = DefaultConfigId,
            DefectId = nextRecord
        });

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var taskId = (int)nextNumber;
        _logger.LogInformation("Created task {TaskId} for tracker user {TtUserId}.", taskId, actor.TtUserId);
        return taskId;
    }

    async Task<decimal> LoadWorkingDispositionAsync(CancellationToken cancellationToken)
    {
        var working = await _db.Dispositions
            .AsNoTracking()
            .Where(d => d.BeingWorked == 1)
            .Select(d => (decimal?)d.IdRecord)
            .FirstOrDefaultAsync(cancellationToken);

        return working ?? 1;
    }

    async Task<TaskActor> ResolveActorAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        TrackerUser? user;
        var ttUserClaim = principal.FindFirst("ttuserid")?.Value;
        if (!string.IsNullOrWhiteSpace(ttUserClaim))
        {
            if (!int.TryParse(ttUserClaim, out var ttUserId) || ttUserId < 1)
            {
                throw new TaskAccessException("The authenticated user is not allowed to create tasks.");
            }

            user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdRecord == ttUserId, cancellationToken);
        }
        else
        {
            var email = EmailClaimTypes
                .Select(type => principal.FindFirst(type)?.Value)
                .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new TaskAccessException("The authenticated user is not allowed to create tasks.");
            }

            var normalized = email.ToUpperInvariant();
            user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToUpper() == normalized, cancellationToken);
        }

        if (user == null || string.IsNullOrWhiteSpace(user.Email))
        {
            throw new TaskAccessException("The authenticated user is not allowed to create tasks.");
        }

        var normalizedEmail = user.Email.ToUpperInvariant();
        var person = await _persons.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.WorkEmail != null && p.WorkEmail.ToUpper() == normalizedEmail, cancellationToken);
        var ttUserIdValue = Convert.ToInt32(user.IdRecord);
        if (person == null || person.Retired || ttUserIdValue < 1)
        {
            throw new TaskAccessException("The authenticated user is not allowed to create tasks.");
        }

        return new TaskActor(ttUserIdValue, person.WorkEmail?.Trim() ?? "");
    }

    async Task<TaskDefaults> LoadDefaultsAsync(CancellationToken cancellationToken)
    {
        var rows = await _db.FieldDefinitions
            .AsNoTracking()
            .Where(f => f.EntityType == DefaultEntityType && DefaultFieldCodes.Contains(f.FieldCode))
            .Select(f => new { f.FieldCode, f.DefaultValue })
            .ToListAsync(cancellationToken);

        var values = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.DefaultValue) || !int.TryParse(row.DefaultValue, out var parsed))
            {
                throw new InvalidOperationException($"Task default '{row.FieldCode}' is not a whole number.");
            }

            values[row.FieldCode] = parsed;
        }

        return new TaskDefaults(
            Require(values, "TYPE"),
            Require(values, "PROD"),
            Require(values, "DISP"),
            Require(values, "PRIO"),
            Require(values, "COMP"),
            Require(values, "SEVR"));
    }

    static int Require(Dictionary<string, int> values, string code)
    {
        if (!values.TryGetValue(code, out var value))
        {
            throw new InvalidOperationException($"Task default '{code}' is not configured.");
        }

        return value;
    }

    sealed record TaskActor(int TtUserId, string Email);

    sealed record TaskDefaults(int Type, int Product, int Disposition, int Priority, int Component, int Severity);
}
