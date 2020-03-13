using Microsoft.EntityFrameworkCore;

namespace ABATON.Models
{
    public class DosageContext : DbContext
    {
        public DosageContext(DbContextOptions<DosageContext> options)
        : base(options)
        {
        }

        public DbSet<Dosage> Dosages { get; set; }

    }
}
