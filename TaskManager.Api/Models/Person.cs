namespace TaskManager.Api.Models;

/// <summary>
/// Person in TASKS.dbo.PERSONS. WORK_EMAIL matches USERS.EMailAddr, and RETIRED blocks task creation.
/// </summary>
public sealed class Person
{
    public int Id { get; set; }

    public string? WorkEmail { get; set; }

    public bool Retired { get; set; }
}
