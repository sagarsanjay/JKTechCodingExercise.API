using JKTechCodingExercise.Core.Enums;

namespace JKTechCodingExercise.Core.Models;

public class IngestionTask
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public IngestionStatus Status { get; set; } = IngestionStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? SpringResponse { get; set; }
}
