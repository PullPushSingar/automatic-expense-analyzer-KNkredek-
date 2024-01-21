
using gmailReaderWebApi.Models;
using Microsoft.EntityFrameworkCore;







namespace gmailReaderWebApi
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<Operation> Operations { get; set; }

        // Konfiguracja modeli i relacji
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Dodatkowe konfiguracje modeli
        }
    }
}
