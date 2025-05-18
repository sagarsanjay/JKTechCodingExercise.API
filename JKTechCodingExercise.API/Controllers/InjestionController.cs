using JKTechCodingExercise.Core.DTO;
using JKTechCodingExercise.Core.Enums;
using JKTechCodingExercise.Core.Interfaces;
using JKTechCodingExercise.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JKTechCodingExercise.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngestionController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;

    public IngestionController(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("trigger")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> TriggerIngestion([FromBody] IngestionRequestDto dto)
    {
        var doc = await _unitOfWork.Documents.GetByIdAsync(dto.DocumentId);
        if (doc == null)
            return NotFound("Document not found");

        var ingestionTask = new IngestionTask
        {
            DocumentId = doc.Id,
            Status = IngestionStatus.Pending
        };

        await _unitOfWork.Repository<IngestionTask>().AddAsync(ingestionTask);
        await _unitOfWork.CompleteAsync();

        var client = _httpClientFactory.CreateClient();

        try
        {
            var springEndpoint = "https://spring-backend.com/api/ingest";
            var response = await client.PostAsJsonAsync(springEndpoint, new { docId = doc.Id });

            ingestionTask.Status = response.IsSuccessStatusCode ? IngestionStatus.InProgress : IngestionStatus.Failed;
            ingestionTask.SpringResponse = await response.Content.ReadAsStringAsync();

            await _unitOfWork.CompleteAsync();

            return Ok(new { taskId = ingestionTask.Id, status = ingestionTask.Status });
        }
        catch (Exception ex)
        {
            ingestionTask.Status = IngestionStatus.Failed;
            ingestionTask.SpringResponse = ex.Message;
            await _unitOfWork.CompleteAsync();

            return StatusCode(500, "Failed to trigger ingestion");
        }
    }
    
    [HttpGet("tasks")]
    [Authorize(Roles = "Admin,Editor,Viewer")]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await _unitOfWork.Repository<IngestionTask>().GetAllAsync();
        return Ok(tasks);
    }
    
    [HttpGet("tasks/{id}")]
    [Authorize(Roles = "Admin,Editor,Viewer")]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await _unitOfWork.Repository<IngestionTask>().GetByIdAsync(id);
        if (task == null)
            return NotFound();
    
        return Ok(task);
    }
    
    [HttpPost("tasks/{id}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CancelTask(int id)
    {
        var task = await _unitOfWork.Repository<IngestionTask>().GetByIdAsync(id);
        if (task == null)
            return NotFound();
    
        if (task.Status == IngestionStatus.Completed || task.Status == IngestionStatus.Failed)
            return BadRequest("Cannot cancel a completed or failed task.");
    
        task.Status = IngestionStatus.Cancelled;
        await _unitOfWork.CompleteAsync();
    
        return Ok("Task cancelled.");
    }
}
