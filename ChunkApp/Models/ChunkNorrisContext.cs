using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ChunkApp.Models
{
    public partial class ChunkNorrisContext : DbContext
    {
        public ChunkNorrisContext()
        {
        }

        public ChunkNorrisContext(DbContextOptions<ChunkNorrisContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Favourite> Favourites { get; set; }
        public virtual DbSet<JokeDetail> JokeDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.\\DEV2022;Initial Catalog=ChunkNorris;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Favourite>(entity =>
            {
                entity.ToTable("Favourite");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<JokeDetail>(entity =>
            {
                entity.HasKey(e => e.OriginalId);

                entity.Property(e => e.OriginalId).HasMaxLength(25);

                entity.Property(e => e.DateCreated).HasColumnType("date");

                entity.Property(e => e.DateModified).HasColumnType("date");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Joke).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
