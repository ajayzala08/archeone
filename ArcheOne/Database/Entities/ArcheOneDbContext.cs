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

	public virtual DbSet<UserDetailsMst> UserDetailsMsts { get; set; }

	public virtual DbSet<UserMst> UserMsts { get; set; }

	//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
	//        => optionsBuilder.UseSqlServer("Server=192.168.1.199,1433;user=sa;password=sa@2022;Database=ArcheOneDB;Trusted_Connection=False;TrustServerCertificate=True;");

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

		modelBuilder.Entity<UserDetailsMst>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK__UserDeta__3214EC072CC579FB");

			entity.ToTable("UserDetailsMst");

			entity.Property(e => e.ConfirmPassword).HasMaxLength(250);
			entity.Property(e => e.ContactNumber).HasMaxLength(250);
			entity.Property(e => e.CreatedDate).HasColumnType("datetime");
			entity.Property(e => e.Dob)
				.HasColumnType("datetime")
				.HasColumnName("DOB");
			entity.Property(e => e.Email).HasMaxLength(250);
			entity.Property(e => e.EmergencyContact).HasMaxLength(250);
			entity.Property(e => e.EmployeeCode).HasMaxLength(250);
			entity.Property(e => e.FirstName).HasMaxLength(250);
			entity.Property(e => e.Gender).HasMaxLength(250);
			entity.Property(e => e.LastName).HasMaxLength(250);
			entity.Property(e => e.MiddleName).HasMaxLength(250);
			entity.Property(e => e.Password).HasMaxLength(250);
			entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
			entity.Property(e => e.UserName).HasMaxLength(250);
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

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
