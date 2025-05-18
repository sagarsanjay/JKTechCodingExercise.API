using JKTechCodingExercise.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace JKTechCodingExercise.Infrastructure.Context;

public class JKTechDBContext : DbContext
{
    public JKTechDBContext(DbContextOptions<JKTechDBContext> options) : base(options) {}
    
    public DbSet<IngestionTask> IngestionTasks { get; set; }

}