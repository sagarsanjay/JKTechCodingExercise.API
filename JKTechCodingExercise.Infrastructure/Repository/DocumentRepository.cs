using JKTechCodingExercise.Core.Interfaces;
using JKTechCodingExercise.Core.Models;
using JKTechCodingExercise.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JKTechCodingExercise.Infrastructure.Repository;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    private readonly JKTechDBContext _context;
    public DocumentRepository(JKTechDBContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Document>> GetByUserAsync(string username)
    {
        return await _context.Documents
            .Where(d => d.UploadedBy == username)
            .ToListAsync();
    }
}
