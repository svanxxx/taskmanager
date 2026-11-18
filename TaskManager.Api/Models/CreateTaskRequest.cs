using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models;

public sealed class CreateTaskRequest
{
    /// <summary>
    /// Task summary. Stored in DEFECTS.Summary, which is nvarchar(255).
    /// An empty value is rejected, matching newTask4MeNow returning without creating a task.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string? Summary { get; set; }
}
