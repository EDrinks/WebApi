using Microsoft.EntityFrameworkCore;

namespace EDrinks.EventSourceSql.Model
{
    public class DomainContext : DbContext
    {
        public DbSet<DomainEvent> DomainEvents { get; set; }

        public DomainContext(DbContextOptions<DomainContext> options) : base(options)
        {
        }
    }
}