using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EcoPoinAPI.Models;

public partial class EcoPoinContext : DbContext
{
    public EcoPoinContext()
    {
    }

    public EcoPoinContext(DbContextOptions<EcoPoinContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Deposit> Deposits { get; set; }

    public virtual DbSet<DepositPoint> DepositPoints { get; set; }

    public virtual DbSet<RedemptionPoint> RedemptionPoints { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<WasteType> WasteTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Integrated Security=true;Database=EcoPoin");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Deposit>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Officer).WithMany(p => p.DepositOfficers).HasConstraintName("FK_Deposits_Officers");

            entity.HasOne(d => d.Resident).WithMany(p => p.DepositResidents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposits_Residents");

            entity.HasOne(d => d.WasteType).WithMany(p => p.Deposits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposits_WasteTypes");
        });

        modelBuilder.Entity<DepositPoint>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Deposit).WithOne(p => p.DepositPoint)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DepositPoints_Deposits");

            entity.HasOne(d => d.Resident).WithMany(p => p.DepositPoints)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DepositPoints_Residents");
        });

        modelBuilder.Entity<RedemptionPoint>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Resident).WithMany(p => p.RedemptionPoints)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RedemptionPoints_Residents");

            entity.HasOne(d => d.Voucher).WithMany(p => p.RedemptionPoints)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RedemptionPoints_Vouchers");
        });

        modelBuilder.Entity<WasteType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WasteType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
