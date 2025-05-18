namespace JKTechCodingExercise.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDocumentRepository Documents { get; }
    IIngestionTaskRepository IngestionTasks { get; }

    IRepository<T> Repository<T>() where T : class;
    Task<int> CompleteAsync();
}

