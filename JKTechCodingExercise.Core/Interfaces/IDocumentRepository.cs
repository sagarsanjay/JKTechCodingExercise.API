using JKTechCodingExercise.Core.Models;

namespace JKTechCodingExercise.Core.Interfaces;

public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetByUserAsync(string username);
}
