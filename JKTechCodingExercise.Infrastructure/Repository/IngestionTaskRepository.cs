using JKTechCodingExercise.Core.Enums;
using JKTechCodingExercise.Core.Interfaces;
using JKTechCodingExercise.Core.Models;
using JKTechCodingExercise.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JKTechCodingExercise.Infrastructure.Repository;

public class IngestionTaskRepository : Repository<IngestionTask>, IIngestionTaskRepository
{
    private readonly JKTechDBContext _context;

    public IngestionTaskRepository(JKTechDBContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<IngestionTask>> GetByStatusAsync(IngestionStatus status)
    {
        return await _context.IngestionTasks
            .Where(t => t.Status == status)
            .ToListAsync();
    }
}
