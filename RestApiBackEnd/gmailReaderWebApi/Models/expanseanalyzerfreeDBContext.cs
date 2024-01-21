using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace gmailReaderWebApi.Models
{
    public partial class expanseanalyzerfreeDBContext : DbContext
    {
        public expanseanalyzerfreeDBContext()
        {
        }

        public expanseanalyzerfreeDBContext(DbContextOptions<expanseanalyzerfreeDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Operation> Operations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=tcp:exapnse-analyzer-freedb.database.windows.net,1433;Initial Catalog=expanse-analyzer-freeDB;Persist Security Info=False;User ID=hubert65119688;Password=%SytnCHxBa5rW81oj$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Operation>(entity =>
            {
                entity.Property(e => e.AccountAmountAfterOperation).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OperationAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OperationDate).HasColumnType("date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
