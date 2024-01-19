using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace biblio.Models;

public partial class BiblioContext : DbContext
{
    public BiblioContext()
    {
    }

    public BiblioContext(DbContextOptions<BiblioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Abonné> Abonnés { get; set; }

    public virtual DbSet<Emprunt> Emprunts { get; set; }

    public virtual DbSet<Livre> Livres { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-04FVHN7\\SQLEXPRESS; Database=BIBLIO;Integrated Security=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abonné>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Abonné__3214EC2772205BEC");

            entity.ToTable("Abonné");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Nom).HasMaxLength(50);
            entity.Property(e => e.Prénom).HasMaxLength(50);
        });

        modelBuilder.Entity<Emprunt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Emprunt__3214EC27DB074D97");

            entity.ToTable("Emprunt");

            entity.HasIndex(e => new { e.LivreId, e.AbonnéId }, "UC_LivreAbonné").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateEmprunt).HasColumnType("datetime");
            entity.Property(e => e.DateRetour).HasColumnType("datetime");

            entity.HasOne(d => d.Abonné).WithMany(p => p.Emprunts)
                .HasForeignKey(d => d.AbonnéId)
                .HasConstraintName("FK__Emprunt__AbonnéI__3C69FB99");

            entity.HasOne(d => d.Livre).WithMany(p => p.Emprunts)
                .HasForeignKey(d => d.LivreId)
                .HasConstraintName("FK__Emprunt__LivreId__3B75D760");
        });

        modelBuilder.Entity<Livre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Livre__3214EC27122EFD75");

            entity.ToTable("Livre");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Auteur).HasMaxLength(50);
            entity.Property(e => e.Resume).HasMaxLength(200);
            entity.Property(e => e.Titre).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
