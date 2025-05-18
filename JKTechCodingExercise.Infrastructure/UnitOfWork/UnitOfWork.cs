using JKTechCodingExercise.Core.Interfaces;
using JKTechCodingExercise.Infrastructure.Context;
using JKTechCodingExercise.Infrastructure.Repository;

namespace JKTechCodingExercise.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly JKTechDBContext _context;

    public IDocumentRepository Documents { get; private set; }
    public IIngestionTaskRepository IngestionTasks { get; private set; }

    public UnitOfWork(JKTechDBContext context)
    {
        _context = context;
        Documents = new DocumentRepository(_context);
        IngestionTasks = new IngestionTaskRepository(_context);
    }

    public IRepository<T> Repository<T>() where T : class
    {
        return new Repository<T>(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose() => _context.Dispose();
}

