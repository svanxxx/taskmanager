namespace TaskManager.Api.Models;

/// <summary>
/// Row in TT_RES.dbo.DEFECTS. Column names and types follow the existing database model.
/// </summary>
public sealed class Defect
{
    public decimal ProjectId { get; set; }
    public decimal IdRecord { get; set; }
    public DateTime? DateCreate { get; set; }
    public decimal? CreatedBy { get; set; }
    public DateTime? DateModify { get; set; }
    public decimal? ModifiedBy { get; set; }
    public decimal? DefectNum { get; set; }
    public string? Summary { get; set; }
    public decimal? Status { get; set; }
    public decimal? InitialStatus { get; set; }
    public decimal? TypeId { get; set; }
    public decimal? ProductId { get; set; }
    public string? Reference { get; set; }
    public decimal? EnteredBy { get; set; }
    public decimal? DispositionId { get; set; }
    public decimal? PriorityId { get; set; }
    public decimal? ComponentId { get; set; }
    public decimal? SeverityId { get; set; }
    public DateTime? DateEnter { get; set; }
    public decimal? AddLocation { get; set; }
    public string? Workaround { get; set; }
    public decimal? TicketId { get; set; }
    public string? Notify { get; set; }
    public int? Order { get; set; }
    public string? Modifier { get; set; }
    public float? Estimate { get; set; }
    public float? Spent { get; set; }
    public string? UserName { get; set; }
    public DateTime? OrderDate { get; set; }
    public int? AssignedUserId { get; set; }
    public string? Branch { get; set; }
    public int? BuildPriority { get; set; }
    public string? BranchBst { get; set; }
    public int? EstimatedBy { get; set; }
    public DateTime? DateTimer { get; set; }
    public int? Attachments { get; set; }
    public string? Version { get; set; }
    public DateTime? Edd { get; set; }
    public int? PrimaryHours { get; set; }
    public int? PrimaryEstimate { get; set; }
}
