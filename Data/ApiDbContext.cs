using Apiaguas.Models;
using Microsoft.EntityFrameworkCore;

namespace Apiaguas.Data;

// Contexto do EF Core: ponte entre a entidade AguaEntity e o banco SQLite.
public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options)
        : base(options)
    {
    }

    public DbSet<AguaEntity> Aguas => Set<AguaEntity>();
}
