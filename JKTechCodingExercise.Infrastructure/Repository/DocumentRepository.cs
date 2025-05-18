using JKTechCodingExercise.Core.Interfaces;
using JKTechCodingExercise.Core.Models;
using JKTechCodingExercise.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JKTechCodingExercise.Infrastructure.Repository;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(JKTechDBContext context) : base(context) { }

    public async Task<IEnumerable<Document>> GetByUserAsync(string username)
    {
        return await _context.Set<Document>()
            .Where(d => d.UploadedBy == username)
            .ToListAsync();
    }
}
