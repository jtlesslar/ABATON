using Microsoft.EntityFrameworkCore;

namespace ABATON.Models
{
    public class DrugContext : DbContext
    {
        public DrugContext(DbContextOptions<DrugContext> options)
        : base(options)
        {
        }

        public DbSet<Drug> Drugs { get; set; }

    }
}
