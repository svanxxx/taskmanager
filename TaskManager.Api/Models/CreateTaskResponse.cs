namespace TaskManager.Api.Models;

public sealed class CreateTaskResponse
{
    /// <summary>
    /// DEFECTS.DefectNum of the created task. This is the integer returned by newTask4MeNow.
    /// </summary>
    public int Id { get; set; }
}
