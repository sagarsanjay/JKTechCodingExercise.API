using JKTechCodingExercise.Core.DTO;
using JKTechCodingExercise.Core.Interfaces;
using JKTechCodingExercise.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JKTechCodingExercise.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<IdentityUser> _userManager;

    public DocumentController(IUnitOfWork unitOfWork, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _env = env;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Editor,Viewer")]
    public async Task<IActionResult> GetAll()
    {
        var docs = await _unitOfWork.Documents.GetAllAsync();
        return Ok(docs);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Editor,Viewer")]
    public async Task<IActionResult> Get(int id)
    {
        var doc = await _unitOfWork.Documents.GetByIdAsync(id);
        if (doc == null)
            return NotFound();
        return Ok(doc);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Upload([FromForm] DocumentDto dto)
    {
        if (dto.File == null || dto.File.Length == 0)
            return BadRequest("File is required");

        var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
        var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var username = User.Identity?.Name ?? "unknown";

        var doc = new Document
        {
            Title = dto.Title,
            Description = dto.Description,
            FilePath = $"/uploads/{fileName}",
            UploadedBy = username
        };

        await _unitOfWork.Documents.AddAsync(doc);
        await _unitOfWork.CompleteAsync();

        return Ok(doc);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Update(int id, [FromBody] Document updated)
    {
        var doc = await _unitOfWork.Documents.GetByIdAsync(id);
        if (doc == null) return NotFound();

        doc.Title = updated.Title;
        doc.Description = updated.Description;
        await _unitOfWork.CompleteAsync();

        return Ok(doc);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var doc = await _unitOfWork.Documents.GetByIdAsync(id);
        if (doc == null) return NotFound();

        var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", doc.FilePath.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        _unitOfWork.Documents.Remove(doc);
        await _unitOfWork.CompleteAsync();

        return Ok("Deleted");
    }
}
