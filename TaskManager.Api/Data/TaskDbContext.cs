using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data;

public sealed class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }

    public DbSet<Defect> Defects => Set<Defect>();

    public DbSet<Disposition> Dispositions => Set<Disposition>();

    public DbSet<TrackerUser> Users => Set<TrackerUser>();

    public DbSet<FieldDefinition> FieldDefinitions => Set<FieldDefinition>();

    public DbSet<ReportBy> Reports => Set<ReportBy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Defect>(entity =>
        {
            entity.ToTable("DEFECTS", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(e => new { e.ProjectId, e.IdRecord });

            entity.Property(e => e.ProjectId).HasColumnName("ProjectID").HasPrecision(10, 0).ValueGeneratedNever();
            entity.Property(e => e.IdRecord).HasColumnName("idRecord").HasPrecision(10, 0).ValueGeneratedNever();
            entity.Property(e => e.DateCreate).HasColumnName("dateCreate").HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasColumnName("idCreateBy").HasPrecision(10, 0);
            entity.Property(e => e.DateModify).HasColumnName("dateModify").HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasColumnName("idModifyBy").HasPrecision(10, 0);
            entity.Property(e => e.DefectNum).HasColumnName("DefectNum").HasPrecision(10, 0);
            entity.Property(e => e.Summary).HasColumnName("Summary").HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("Status").HasPrecision(10, 0);
            entity.Property(e => e.InitialStatus).HasColumnName("InitStatus").HasPrecision(10, 0);
            entity.Property(e => e.TypeId).HasColumnName("idType").HasPrecision(10, 0);
            entity.Property(e => e.ProductId).HasColumnName("idProduct").HasPrecision(10, 0);
            entity.Property(e => e.Reference).HasColumnName("Reference").HasMaxLength(255);
            entity.Property(e => e.EnteredBy).HasColumnName("idEnterBy").HasPrecision(10, 0);
            entity.Property(e => e.DispositionId).HasColumnName("idDisposit").HasPrecision(10, 0);
            entity.Property(e => e.PriorityId).HasColumnName("idPriority").HasPrecision(10, 0);
            entity.Property(e => e.ComponentId).HasColumnName("idCompon").HasPrecision(10, 0);
            entity.Property(e => e.SeverityId).HasColumnName("idSeverity").HasPrecision(10, 0);
            entity.Property(e => e.DateEnter).HasColumnName("dateEnter").HasColumnType("datetime");
            entity.Property(e => e.AddLocation).HasColumnName("AddLocat").HasPrecision(5, 0);
            entity.Property(e => e.Workaround).HasColumnName("Workaround").HasColumnType("ntext");
            entity.Property(e => e.TicketId).HasColumnName("idTicket").HasPrecision(10, 0);
            entity.Property(e => e.Notify).HasColumnName("Notify").HasMaxLength(4000).IsUnicode(false);
            entity.Property(e => e.Order).HasColumnName("iOrder");
            entity.Property(e => e.Modifier).HasColumnName("sModifier").HasMaxLength(50).IsFixedLength();
            entity.Property(e => e.Estimate).HasColumnName("Estim").HasColumnType("real");
            entity.Property(e => e.Spent).HasColumnName("Spent").HasColumnType("real");
            entity.Property(e => e.UserName).HasColumnName("Usr").HasMaxLength(50).IsFixedLength();
            entity.Property(e => e.OrderDate).HasColumnName("IOrderDate").HasColumnType("datetime");
            entity.Property(e => e.AssignedUserId).HasColumnName("idUsr");
            entity.Property(e => e.Branch).HasColumnName("branch").HasMaxLength(256).IsFixedLength();
            entity.Property(e => e.BuildPriority).HasColumnName("iBuildPriority");
            entity.Property(e => e.BranchBst).HasColumnName("branchBST").HasMaxLength(256).IsFixedLength();
            entity.Property(e => e.EstimatedBy).HasColumnName("idEstim");
            entity.Property(e => e.DateTimer).HasColumnName("dateTimer").HasColumnType("datetime");
            entity.Property(e => e.Attachments).HasColumnName("attachs");
            entity.Property(e => e.Version).HasColumnName("Version").HasMaxLength(128).IsFixedLength();
            entity.Property(e => e.Edd).HasColumnName("EDD").HasColumnType("datetime");
            entity.Property(e => e.PrimaryHours).HasColumnName("PrimaryHours");
            entity.Property(e => e.PrimaryEstimate).HasColumnName("PrimaryEstim");

            // Columns the legacy insert left unset stay out of the INSERT so database defaults still apply.
            foreach (var property in new PropertyBuilder[]
            {
                entity.Property(e => e.Reference),
                entity.Property(e => e.Workaround),
                entity.Property(e => e.TicketId),
                entity.Property(e => e.Notify),
                entity.Property(e => e.Spent),
                entity.Property(e => e.UserName),
                entity.Property(e => e.OrderDate),
                entity.Property(e => e.Branch),
                entity.Property(e => e.BuildPriority),
                entity.Property(e => e.BranchBst),
                entity.Property(e => e.EstimatedBy),
                entity.Property(e => e.DateTimer),
                entity.Property(e => e.Attachments),
                entity.Property(e => e.Version),
                entity.Property(e => e.Edd),
                entity.Property(e => e.PrimaryHours),
                entity.Property(e => e.PrimaryEstimate)
            })
            {
                property.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                property.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
            }
        });

        modelBuilder.Entity<TrackerUser>(entity =>
        {
            entity.ToTable("USERS", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(e => e.IdRecord);

            entity.Property(e => e.IdRecord).HasColumnName("idRecord").HasPrecision(10, 0).ValueGeneratedNever();
            entity.Property(e => e.Email).HasColumnName("EMailAddr");
        });

        modelBuilder.Entity<FieldDefinition>(entity =>
        {
            entity.ToTable("FLDDFNTN", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(e => new { e.EntityType, e.FieldCode });

            entity.Property(e => e.EntityType).HasColumnName("EntityType");
            entity.Property(e => e.FieldCode).HasColumnName("FieldCode");
            entity.Property(e => e.DefaultValue).HasColumnName("DefaultVal");
        });

        modelBuilder.Entity<ReportBy>(entity =>
        {
            entity.ToTable("REPORTBY", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(e => e.IdRecord);

            entity.Property(e => e.IdRecord).HasColumnName("IDRECORD").HasPrecision(10, 0).ValueGeneratedNever();
            entity.Property(e => e.ProjectId).HasColumnName("PROJECTID").HasPrecision(10, 0);
            entity.Property(e => e.FoundBy).HasColumnName("IDFOUNDBY").HasPrecision(10, 0);
            entity.Property(e => e.DateFound).HasColumnName("DATEFOUND").HasColumnType("datetime");
            entity.Property(e => e.OrderNum).HasColumnName("ORDERNUM").HasPrecision(10, 0);
            entity.Property(e => e.ReproducedId).HasColumnName("IDREPROD").HasPrecision(10, 0);
            entity.Property(e => e.TestConditionType).HasColumnName("TSTCONTYPE").HasPrecision(10, 0);
            entity.Property(e => e.ConfigId).HasColumnName("IDCONFIG").HasPrecision(10, 0);
            entity.Property(e => e.DefectId).HasColumnName("IDDEFREC").HasPrecision(10, 0);
        });

        modelBuilder.Entity<Disposition>(entity =>
        {
            entity.ToTable("FLDDISPO", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(e => new { e.IdRecord, e.ProjectId });

            entity.Property(e => e.IdRecord).HasColumnName("idRecord").HasPrecision(10, 0).ValueGeneratedNever();
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID").HasPrecision(10, 0).ValueGeneratedNever();
            entity.Property(e => e.FieldOrder).HasColumnName("FieldOrder").HasPrecision(10, 0);
            entity.Property(e => e.Descriptor).HasColumnName("Descriptor").HasMaxLength(255);
            entity.Property(e => e.Color).HasColumnName("Color").HasMaxLength(255);
            entity.Property(e => e.RequireWork).HasColumnName("RequireWork");
            entity.Property(e => e.BeingWorked).HasColumnName("BeingWorked");
            entity.Property(e => e.CannotStart).HasColumnName("CannotStart");
            entity.Property(e => e.TestsPassed).HasColumnName("TestsPassed");
            entity.Property(e => e.TestsRejected).HasColumnName("TestsRejected");
            entity.Property(e => e.TestsStarted).HasColumnName("TestsStarted");
        });
    }
}
