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

    public virtual DbSet<CandidateMst> CandidateMsts { get; set; }

    public virtual DbSet<ClientMst> ClientMsts { get; set; }

    public virtual DbSet<CompanyMst> CompanyMsts { get; set; }

    public virtual DbSet<DefaultPermission> DefaultPermissions { get; set; }

    public virtual DbSet<EmploymentTypeMst> EmploymentTypeMsts { get; set; }

    public virtual DbSet<HireMst> HireMsts { get; set; }

    public virtual DbSet<HireStatusMst> HireStatusMsts { get; set; }

    public virtual DbSet<InterviewStatusMst> InterviewStatusMsts { get; set; }

    public virtual DbSet<InterviewTypeStatusMst> InterviewTypeStatusMsts { get; set; }

    public virtual DbSet<LinkMst> LinkMsts { get; set; }

    public virtual DbSet<OfferMst> OfferMsts { get; set; }

    public virtual DbSet<OfferStatusMst> OfferStatusMsts { get; set; }

    public virtual DbSet<PermissionMst> PermissionMsts { get; set; }

    public virtual DbSet<PositionTypeMst> PositionTypeMsts { get; set; }

    public virtual DbSet<RequirementForMst> RequirementForMsts { get; set; }

    public virtual DbSet<RequirementMst> RequirementMsts { get; set; }

    public virtual DbSet<RequirementTypeMst> RequirementTypeMsts { get; set; }

    public virtual DbSet<ResumeFileUploadDetailMst> ResumeFileUploadDetailMsts { get; set; }

    public virtual DbSet<RoleMst> RoleMsts { get; set; }

    public virtual DbSet<ScheduleInterviewMst> ScheduleInterviewMsts { get; set; }

    public virtual DbSet<UserMst> UserMsts { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.1.199,1433;user=sa;password=sa@2022;Database=ArcheOneDB;Encrypt=False;Trusted_Connection=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CandidateMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Candidat__3214EC0782B37697");

            entity.ToTable("CandidateMst");

            entity.Property(e => e.AadharNumber).HasMaxLength(50);
            entity.Property(e => e.AlternateNumber).HasMaxLength(20);
            entity.Property(e => e.BankAccountNo).HasMaxLength(100);
            entity.Property(e => e.BanlName).HasMaxLength(100);
            entity.Property(e => e.Branch).HasMaxLength(100);
            entity.Property(e => e.CandidateName).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(100);
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Ctc).HasColumnName("CTC");
            entity.Property(e => e.CurrentAddress).HasMaxLength(100);
            entity.Property(e => e.CurrentDesignation).HasMaxLength(20);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Ectc).HasColumnName("ECTC");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.EmergencyContact).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.Gp).HasColumnName("GP");
            entity.Property(e => e.Ifsccode)
                .HasMaxLength(100)
                .HasColumnName("IFSCCode");
            entity.Property(e => e.JoiningDate).HasColumnType("datetime");
            entity.Property(e => e.JoiningLocation).HasMaxLength(100);
            entity.Property(e => e.MaritalStatus).HasMaxLength(20);
            entity.Property(e => e.OfferDate).HasColumnType("datetime");
            entity.Property(e => e.OfferDesignation).HasMaxLength(20);
            entity.Property(e => e.PanNumber).HasMaxLength(50);
            entity.Property(e => e.PermanentAddress).HasMaxLength(100);
            entity.Property(e => e.SelectionDate).HasColumnType("datetime");
            entity.Property(e => e.Skill).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ClientMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClientMs__3214EC078D69D699");

            entity.ToTable("ClientMst");

            entity.Property(e => e.ClientName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmailId).HasMaxLength(100);
            entity.Property(e => e.MobileNo).HasMaxLength(20);
            entity.Property(e => e.PocemailIdPrimary)
                .HasMaxLength(100)
                .HasColumnName("POCEmailIdPrimary");
            entity.Property(e => e.PocemailIdSecondary)
                .HasMaxLength(100)
                .HasColumnName("POCEmailIdSecondary");
            entity.Property(e => e.PocnamePrimary)
                .HasMaxLength(100)
                .HasColumnName("POCNamePrimary");
            entity.Property(e => e.PocnameSecondary)
                .HasMaxLength(100)
                .HasColumnName("POCNameSecondary");
            entity.Property(e => e.PocnumberPrimary)
                .HasMaxLength(20)
                .HasColumnName("POCNumberPrimary");
            entity.Property(e => e.PocnumberSecondary)
                .HasMaxLength(20)
                .HasColumnName("POCNumberSecondary");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

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

        modelBuilder.Entity<EmploymentTypeMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employme__3214EC0739B3CF02");

            entity.ToTable("EmploymentTypeMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmploymentTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HireMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HireMst__3214EC07CAC7E046");

            entity.ToTable("HireMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HireStatusMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HireStat__3214EC0736A01457");

            entity.ToTable("HireStatusMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.HireStatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InterviewStatusMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC070D4C3671");

            entity.ToTable("InterviewStatusMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InterviewStatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InterviewTypeStatusMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC07C893191D");

            entity.ToTable("InterviewTypeStatusMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InterviewStatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<LinkMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LinkMst__3214EC078FAB29B8");

            entity.ToTable("LinkMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OfferMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OfferMst__3214EC078C418C3B");

            entity.ToTable("OfferMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OfferStatusMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OfferSta__3214EC079099541F");

            entity.ToTable("OfferStatusMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OfferStatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
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

        modelBuilder.Entity<PositionTypeMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Position__3214EC0765018B46");

            entity.ToTable("PositionTypeMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PositionTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequirementForMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requirem__3214EC070C211281");

            entity.ToTable("RequirementForMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RequirementForName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequirementMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requirem__3214EC07CB9E09D2");

            entity.ToTable("RequirementMst");

            entity.Property(e => e.BillRate).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndClient).HasMaxLength(50);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.JobCode).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.MainSkill).HasMaxLength(50);
            entity.Property(e => e.MandatorySkills).HasMaxLength(100);
            entity.Property(e => e.PayRate).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Pocname)
                .HasMaxLength(100)
                .HasColumnName("POCName");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequirementTypeMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requirem__3214EC07DCFBD75D");

            entity.ToTable("RequirementTypeMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RequirementTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ResumeFileUploadDetailMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ResumeFi__3214EC072A1D700D");

            entity.ToTable("ResumeFileUploadDetailMst");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.ApplicantDob)
                .HasColumnType("datetime")
                .HasColumnName("ApplicantDOB");
            entity.Property(e => e.ApplicantName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Ctc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("CTC");
            entity.Property(e => e.CurrentCompany).HasMaxLength(100);
            entity.Property(e => e.CurrentDesignation).HasMaxLength(100);
            entity.Property(e => e.CurrentDrawing).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CurrentLocation).HasMaxLength(100);
            entity.Property(e => e.CurrentTakeHome).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Ectc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("ECTC");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.ExpectedDrawing).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ExpectedTakeHome).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.F2favaillability).HasColumnName("F2FAvaillability");
            entity.Property(e => e.HikeReason).HasMaxLength(100);
            entity.Property(e => e.HowJoinEarlyReason).HasMaxLength(100);
            entity.Property(e => e.LastSalaryHike).HasColumnType("datetime");
            entity.Property(e => e.Native).HasMaxLength(100);
            entity.Property(e => e.PanNumber).HasMaxLength(100);
            entity.Property(e => e.PrefferedLocation).HasMaxLength(100);
            entity.Property(e => e.ReasonForJoin).HasMaxLength(100);
            entity.Property(e => e.ReasonGap).HasMaxLength(100);
            entity.Property(e => e.ReasonOfRelocation).HasMaxLength(100);
            entity.Property(e => e.ResumeName).HasMaxLength(50);
            entity.Property(e => e.TeliPhonicInTime).HasMaxLength(20);
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

        modelBuilder.Entity<ScheduleInterviewMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC075726AC4E");

            entity.ToTable("ScheduleInterviewMst");

            entity.Property(e => e.CandidateName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InterviewBy).HasMaxLength(100);
            entity.Property(e => e.InterviewDate).HasColumnType("datetime");
            entity.Property(e => e.InterviewLocation).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(50);
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
