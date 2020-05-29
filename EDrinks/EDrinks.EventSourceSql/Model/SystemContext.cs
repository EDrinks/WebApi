using Microsoft.EntityFrameworkCore;

namespace EDrinks.EventSourceSql.Model
{
    public class SystemContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        
        public SystemContext(DbContextOptions<SystemContext> options) : base(options)
        {
        }
    }
}