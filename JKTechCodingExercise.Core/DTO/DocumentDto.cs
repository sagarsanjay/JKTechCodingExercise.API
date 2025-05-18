using Microsoft.AspNetCore.Http;

namespace JKTechCodingExercise.Core.DTO;

public class DocumentDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IFormFile File { get; set; }
}

public class DocumentResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FilePath { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; }
}
