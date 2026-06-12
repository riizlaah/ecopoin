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
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualWeight)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("actualWeight");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.EstimatedWeight)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("estimatedWeight");
            entity.Property(e => e.Notes)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("notes");
            entity.Property(e => e.OfficerId).HasColumnName("officerId");
            entity.Property(e => e.PhotoPath)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("photoPath");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("rejectionReason");
            entity.Property(e => e.ResidentId).HasColumnName("residentId");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.WasteTypeId).HasColumnName("wasteTypeId");

            entity.HasOne(d => d.Officer).WithMany(p => p.DepositOfficers)
                .HasForeignKey(d => d.OfficerId)
                .HasConstraintName("FK_Deposits_Officers");

            entity.HasOne(d => d.Resident).WithMany(p => p.Deposits)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposits_Residents");

            entity.HasOne(d => d.WasteType).WithMany(p => p.Deposits)
                .HasForeignKey(d => d.WasteTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposits_WasteTypes");
        });

        modelBuilder.Entity<DepositPoint>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("int")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.DepositId).HasColumnName("depositId");
            entity.Property(e => e.PointTariff)
                .HasColumnType("int")
                .HasColumnName("pointTariff");
            entity.Property(e => e.ResidentId).HasColumnName("residentId");

            entity.HasOne(d => d.Deposit).WithOne(p => p.DepositPoint)
                .HasForeignKey<DepositPoint>(dp => dp.DepositId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DepositPoints_Deposits");

            entity.HasOne(d => d.Resident).WithMany(p => p.DepositPoints)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DepositPoints_Residents");
        });

        modelBuilder.Entity<RedemptionPoint>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("int")
                .HasColumnName("amount");
            entity.Property(e => e.Code)
                .HasMaxLength(72)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsUsed).HasColumnName("isUsed");
            entity.Property(e => e.PointCost)
                .HasColumnType("int")
                .HasColumnName("pointCost");
            entity.Property(e => e.ResidentId).HasColumnName("residentId");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.VoucherId).HasColumnName("voucherId");

            entity.HasOne(d => d.Resident).WithMany(p => p.RedemptionPoints)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RedemptionPoints_Residents");

            entity.HasOne(d => d.Voucher).WithMany(p => p.RedemptionPoints)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RedemptionPoints_Vouchers");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("fullName");
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(24)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PointCost)
                .HasColumnType("int")
                .HasColumnName("pointCost");
        });

        modelBuilder.Entity<WasteType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WasteType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Co2factor)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CO2Factor");
            entity.Property(e => e.Code)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PointTariff)
                .HasColumnType("int")
                .HasColumnName("pointTariff");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
