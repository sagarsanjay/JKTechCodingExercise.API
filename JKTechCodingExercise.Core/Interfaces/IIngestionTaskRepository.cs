using JKTechCodingExercise.Core.Enums;
using JKTechCodingExercise.Core.Models;

namespace JKTechCodingExercise.Core.Interfaces;

public interface IIngestionTaskRepository : IRepository<IngestionTask>
{
    Task<IEnumerable<IngestionTask>> GetByStatusAsync(IngestionStatus status);
}
