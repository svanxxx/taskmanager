using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public sealed class TasksController : ControllerBase
{
    static readonly SemaphoreSlim CreateLock = new(1, 1);

    readonly TaskCreationService _tasks;

    public TasksController(TaskCreationService tasks)
    {
        _tasks = tasks;
    }

    /// <summary>
    /// Creates a task for the authenticated user and returns the new task id.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CreateTaskResponse>> Create(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        await CreateLock.WaitAsync(cancellationToken);
        try
        {
            var id = await _tasks.CreateAsync(request.Summary, User, cancellationToken);
            return Created($"/api/tasks/{id}", new CreateTaskResponse { Id = id });
        }
        finally
        {
            CreateLock.Release();
        }
    }
}
