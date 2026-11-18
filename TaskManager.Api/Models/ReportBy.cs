namespace TaskManager.Api.Models;

/// <summary>
/// Row inserted into TT_RES.dbo.REPORTBY for each new defect.
/// </summary>
public sealed class ReportBy
{
    public decimal IdRecord { get; set; }

    public decimal ProjectId { get; set; }

    public decimal FoundBy { get; set; }

    public DateTime? DateFound { get; set; }

    public decimal OrderNum { get; set; }

    public decimal ReproducedId { get; set; }

    public decimal TestConditionType { get; set; }

    public decimal ConfigId { get; set; }

    public decimal DefectId { get; set; }
}
