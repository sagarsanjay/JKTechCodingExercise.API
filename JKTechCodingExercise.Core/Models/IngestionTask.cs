using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JKTechCodingExercise.Core.Enums;

namespace JKTechCodingExercise.Core.Models;

[Table("IngestionTask", Schema = "CodingExercise")]
public class IngestionTask
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int DocumentId { get; set; }
    
    public IngestionStatus Status { get; set; } = IngestionStatus.Pending;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? SpringResponse { get; set; }
}
