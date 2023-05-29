using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ArcheOne.Database.Entities;

public partial class ArcheOneDbContext : DbContext
{
    public ArcheOneDbContext()
    {
    }

    public ArcheOneDbContext(DbContextOptions<ArcheOneDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CompanyMst> CompanyMsts { get; set; }

    public virtual DbSet<DefaultPermission> DefaultPermissions { get; set; }

    public virtual DbSet<LinkMst> LinkMsts { get; set; }

    public virtual DbSet<PermissionMst> PermissionMsts { get; set; }

    public virtual DbSet<RoleMst> RoleMsts { get; set; }

    public virtual DbSet<UserMst> UserMsts { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=192.168.1.199,1433;user=sa;password=sa@2022;Database=ArcheOneDB;Encrypt=False;Trusted_Connection=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompanyMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CompanyM__3214EC0797341D38");

            entity.ToTable("CompanyMst");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CompanyName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.LogoUrl).HasColumnName("LogoURL");
            entity.Property(e => e.Mobile1).HasMaxLength(20);
            entity.Property(e => e.Mobile2).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Pincode).HasMaxLength(10);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Website).HasMaxLength(100);
        });

        modelBuilder.Entity<DefaultPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DefaultP__3214EC0727B6322F");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<LinkMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LinkMst__3214EC078FAB29B8");

            entity.ToTable("LinkMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PermissionMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC07A9F2016E");

            entity.ToTable("PermissionMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PermissionCode).HasMaxLength(100);
            entity.Property(e => e.PermissionName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RoleMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoleMst__3214EC07C7BEF5BC");

            entity.ToTable("RoleMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RoleCode).HasMaxLength(100);
            entity.Property(e => e.RoleName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserMst__3214EC07721E0996");

            entity.ToTable("UserMst");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.Mobile1).HasMaxLength(20);
            entity.Property(e => e.Mobile2).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.PhotoUrl).HasColumnName("PhotoURL");
            entity.Property(e => e.Pincode).HasMaxLength(10);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserPerm__3214EC0754C03AE1");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
