namespace TaskManager.Api.Models;

/// <summary>
/// Tracker account in TT_RES.dbo.USERS. Only the columns needed to resolve the signed-in user are mapped.
/// </summary>
public sealed class TrackerUser
{
    public decimal IdRecord { get; set; }

    public string? Email { get; set; }
}
