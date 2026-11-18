namespace TaskManager.Api.Models;

/// <summary>
/// Row in TT_RES.dbo.FLDDISPO. BeingWorked identifies the disposition used by newTask4MeNow.
/// </summary>
public sealed class Disposition
{
    public decimal IdRecord { get; set; }
    public decimal ProjectId { get; set; }
    public decimal? FieldOrder { get; set; }
    public string? Descriptor { get; set; }
    public string? Color { get; set; }
    public int RequireWork { get; set; }
    public int BeingWorked { get; set; }
    public int CannotStart { get; set; }
    public int TestsPassed { get; set; }
    public int TestsRejected { get; set; }
    public int TestsStarted { get; set; }
}
