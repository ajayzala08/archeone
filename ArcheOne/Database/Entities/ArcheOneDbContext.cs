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

    public virtual DbSet<AppraisalMst> AppraisalMsts { get; set; }

    public virtual DbSet<AppraisalRatingMst> AppraisalRatingMsts { get; set; }

    public virtual DbSet<CalenderYearMst> CalenderYearMsts { get; set; }

    public virtual DbSet<CandidateMst> CandidateMsts { get; set; }

    public virtual DbSet<ClientMst> ClientMsts { get; set; }

    public virtual DbSet<CompanyMst> CompanyMsts { get; set; }

    public virtual DbSet<DailyTaskMst> DailyTaskMsts { get; set; }

    public virtual DbSet<DefaultPermission> DefaultPermissions { get; set; }

    public virtual DbSet<DepartmentMst> DepartmentMsts { get; set; }

    public virtual DbSet<DesignationMst> DesignationMsts { get; set; }

    public virtual DbSet<DocumentTypeMst> DocumentTypeMsts { get; set; }

    public virtual DbSet<EmploymentTypeMst> EmploymentTypeMsts { get; set; }

    public virtual DbSet<FinancialYearMst> FinancialYearMsts { get; set; }

    public virtual DbSet<HireStatusMst> HireStatusMsts { get; set; }

    public virtual DbSet<HolidayMst> HolidayMsts { get; set; }

    public virtual DbSet<InterviewMst> InterviewMsts { get; set; }

    public virtual DbSet<InterviewRoundMst> InterviewRoundMsts { get; set; }

    public virtual DbSet<InterviewRoundStatusMst> InterviewRoundStatusMsts { get; set; }

    public virtual DbSet<InterviewRoundTypeMst> InterviewRoundTypeMsts { get; set; }

    public virtual DbSet<LeaveBalanceMst> LeaveBalanceMsts { get; set; }

    public virtual DbSet<LeaveMst> LeaveMsts { get; set; }

    public virtual DbSet<LeaveStatusMst> LeaveStatusMsts { get; set; }

    public virtual DbSet<LeaveTypeMst> LeaveTypeMsts { get; set; }

    public virtual DbSet<LinkMst> LinkMsts { get; set; }

    public virtual DbSet<OfferStatusMst> OfferStatusMsts { get; set; }

    public virtual DbSet<PermissionMst> PermissionMsts { get; set; }

    public virtual DbSet<PolicyMst> PolicyMsts { get; set; }

    public virtual DbSet<PositionTypeMst> PositionTypeMsts { get; set; }

    public virtual DbSet<ProjectMst> ProjectMsts { get; set; }

    public virtual DbSet<ReportingManagerMst> ReportingManagerMsts { get; set; }

    public virtual DbSet<RequirementForMst> RequirementForMsts { get; set; }

    public virtual DbSet<RequirementMst> RequirementMsts { get; set; }

    public virtual DbSet<RequirementStatusMst> RequirementStatusMsts { get; set; }

    public virtual DbSet<RequirementTypeMst> RequirementTypeMsts { get; set; }

    public virtual DbSet<ResumeFileUploadDetailMst> ResumeFileUploadDetailMsts { get; set; }

    public virtual DbSet<RoleMst> RoleMsts { get; set; }

    public virtual DbSet<SalaryMst> SalaryMsts { get; set; }

    public virtual DbSet<SalesContactPersonMst> SalesContactPersonMsts { get; set; }

    public virtual DbSet<SalesLeadActionMst> SalesLeadActionMsts { get; set; }

    public virtual DbSet<SalesLeadFollowUpMst> SalesLeadFollowUpMsts { get; set; }

    public virtual DbSet<SalesLeadMst> SalesLeadMsts { get; set; }

    public virtual DbSet<SalesLeadStatusMst> SalesLeadStatusMsts { get; set; }

    public virtual DbSet<TeamMst> TeamMsts { get; set; }

    public virtual DbSet<UserDetailsMst> UserDetailsMsts { get; set; }

    public virtual DbSet<UserDocumentMst> UserDocumentMsts { get; set; }

    public virtual DbSet<UserMst> UserMsts { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=192.168.1.199,1433;user=sa;password=sa@2022;Database=ArcheOneDB;Encrypt=False;Trusted_Connection=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppraisalMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appraisa__3214EC07F88330DD");

            entity.ToTable("AppraisalMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Year)
                .HasMaxLength(100)
                .HasColumnName("year");
        });

        modelBuilder.Entity<AppraisalRatingMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appraisa__3214EC07E9AA009B");

            entity.ToTable("AppraisalRatingMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GoalNtarget).HasColumnName("GoalNTarget");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CalenderYearMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Calender__3214EC07A0CA3F11");

            entity.ToTable("CalenderYearMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.YearCode).HasMaxLength(100);
            entity.Property(e => e.YearName).HasMaxLength(100);
        });

        modelBuilder.Entity<CandidateMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Candidat__3214EC077B83A013");

            entity.ToTable("CandidateMst");

            entity.Property(e => e.AadharNumber).HasMaxLength(50);
            entity.Property(e => e.BankAccountNo).HasMaxLength(100);
            entity.Property(e => e.BankBranch).HasMaxLength(100);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Ctc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("CTC");
            entity.Property(e => e.CurrentAddress).HasMaxLength(500);
            entity.Property(e => e.CurrentDesignation).HasMaxLength(50);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Ectc)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("ECTC");
            entity.Property(e => e.Email1).HasMaxLength(50);
            entity.Property(e => e.Email2).HasMaxLength(50);
            entity.Property(e => e.EmergencyContact).HasMaxLength(20);
            entity.Property(e => e.EndClient).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.Gp)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("GP");
            entity.Property(e => e.Ifsccode)
                .HasMaxLength(100)
                .HasColumnName("IFSCCode");
            entity.Property(e => e.JoiningDate).HasColumnType("datetime");
            entity.Property(e => e.JoiningLocation).HasMaxLength(500);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MarginPercentage).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MaritalStatus).HasMaxLength(20);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.Mobile1).HasMaxLength(20);
            entity.Property(e => e.Mobile2).HasMaxLength(20);
            entity.Property(e => e.OfferDate).HasColumnType("datetime");
            entity.Property(e => e.OfferDesignation).HasMaxLength(50);
            entity.Property(e => e.PanNumber).HasMaxLength(50);
            entity.Property(e => e.PermanentAddress).HasMaxLength(500);
            entity.Property(e => e.RelevantExperience).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SelectionDate).HasColumnType("datetime");
            entity.Property(e => e.TotalExperience).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ClientMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClientMs__3214EC07DC9E4578");

            entity.ToTable("ClientMst");

            entity.Property(e => e.ClientCode).HasMaxLength(100);
            entity.Property(e => e.ClientName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmailId).HasMaxLength(100);
            entity.Property(e => e.MobileNo).HasMaxLength(20);
            entity.Property(e => e.PocemailPrimary)
                .HasMaxLength(100)
                .HasColumnName("POCEmailPrimary");
            entity.Property(e => e.PocemailSecondary)
                .HasMaxLength(100)
                .HasColumnName("POCEmailSecondary");
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

        modelBuilder.Entity<DailyTaskMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DailyTas__3214EC07BC9B704D");

            entity.ToTable("DailyTaskMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TaskDate).HasColumnType("datetime");
            entity.Property(e => e.TaskStatus).HasMaxLength(30);
            entity.Property(e => e.TimeSpent).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DefaultPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DefaultP__3214EC0727B6322F");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DepartmentMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC073453B577");

            entity.ToTable("DepartmentMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentCode).HasMaxLength(100);
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DesignationMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Designat__3214EC075FB2E8AD");

            entity.ToTable("DesignationMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Designation).HasMaxLength(100);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DocumentTypeMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Document__3214EC0734E26190");

            entity.ToTable("DocumentTypeMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmploymentTypeMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employme__3214EC07835A6743");

            entity.ToTable("EmploymentTypeMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmploymentTypeCode).HasMaxLength(100);
            entity.Property(e => e.EmploymentTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FinancialYearMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Financia__3214EC0750C50CAD");

            entity.ToTable("FinancialYearMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.YearCode).HasMaxLength(100);
            entity.Property(e => e.YearName).HasMaxLength(100);
        });

        modelBuilder.Entity<HireStatusMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HireStat__3214EC07D5B3310A");

            entity.ToTable("HireStatusMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.HireStatusCode).HasMaxLength(100);
            entity.Property(e => e.HireStatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HolidayMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HolidayM__3214EC0744F1A126");

            entity.ToTable("HolidayMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.HolidayDate).HasColumnType("datetime");
            entity.Property(e => e.HolidayName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InterviewMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC07926A24E8");

            entity.ToTable("InterviewMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InterviewRoundMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC079B799760");

            entity.ToTable("InterviewRoundMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InterviewEndDateTime).HasColumnType("datetime");
            entity.Property(e => e.InterviewLocation).HasMaxLength(100);
            entity.Property(e => e.InterviewStartDateTime).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InterviewRoundStatusMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC07649CE71A");

            entity.ToTable("InterviewRoundStatusMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InterviewRoundStatusCode).HasMaxLength(100);
            entity.Property(e => e.InterviewRoundStatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InterviewRoundTypeMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Intervie__3214EC07A68331B2");

            entity.ToTable("InterviewRoundTypeMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InterviewRoundTypeCode).HasMaxLength(100);
            entity.Property(e => e.InterviewRoundTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<LeaveBalanceMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveBal__3214EC07D820450D");

            entity.ToTable("LeaveBalanceMst");

            entity.Property(e => e.AvailableLeaveBalance).HasColumnType("decimal(38, 17)");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LeaveStatus).HasMaxLength(100);
            entity.Property(e => e.PaidDayLeaves).HasColumnType("decimal(38, 17)");
            entity.Property(e => e.PendingLeaveBalance).HasColumnType("decimal(38, 17)");
            entity.Property(e => e.TotalLeaveBalance).HasColumnType("decimal(38, 17)");
            entity.Property(e => e.UnPaidDayLeaves).HasColumnType("decimal(38, 17)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<LeaveMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveMst__3214EC07DB44B77C");

            entity.ToTable("LeaveMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.LeaveBalance).HasColumnType("decimal(38, 17)");
            entity.Property(e => e.NoOfDays).HasColumnType("decimal(38, 17)");
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<LeaveStatusMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveSta__3214EC077F8BB32C");

            entity.ToTable("LeaveStatusMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LeaveStatus).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<LeaveTypeMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveTyp__3214EC07A30DAF6B");

            entity.ToTable("LeaveTypeMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LeaveDays).HasColumnType("decimal(38, 17)");
            entity.Property(e => e.LeaveTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<LinkMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LinkMst__3214EC078FAB29B8");

            entity.ToTable("LinkMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OfferStatusMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OfferSta__3214EC0770B547B3");

            entity.ToTable("OfferStatusMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OfferStatusCode).HasMaxLength(100);
            entity.Property(e => e.OfferStatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PermissionMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC0759F00F12");

            entity.ToTable("PermissionMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PermissionCode).HasMaxLength(100);
            entity.Property(e => e.PermissionName).HasMaxLength(100);
            entity.Property(e => e.PermissionRoute).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PolicyMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PolicyMs__3214EC077BBDF40E");

            entity.ToTable("PolicyMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PolicyDocumentName).HasMaxLength(100);
            entity.Property(e => e.PolicyName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PositionTypeMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Position__3214EC078B131D10");

            entity.ToTable("PositionTypeMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PositionTypeCode).HasMaxLength(100);
            entity.Property(e => e.PositionTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectM__3214EC07CA345D17");

            entity.ToTable("ProjectMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectName).HasMaxLength(100);
            entity.Property(e => e.ProjectStatus).HasMaxLength(30);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ReportingManagerMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reportin__3214EC07A9AB32E0");

            entity.ToTable("ReportingManagerMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ReportingManager).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequirementForMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requirem__3214EC0797DE3239");

            entity.ToTable("RequirementForMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RequirementForCode).HasMaxLength(100);
            entity.Property(e => e.RequirementForName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequirementMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requirem__3214EC078F3066B5");

            entity.ToTable("RequirementMst");

            entity.Property(e => e.AssignedUserIds).HasMaxLength(100);
            entity.Property(e => e.CandidatePayRate).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ClientBillRate).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndClient).HasMaxLength(100);
            entity.Property(e => e.JobCode).HasMaxLength(100);
            entity.Property(e => e.Pocname)
                .HasMaxLength(100)
                .HasColumnName("POCName");
            entity.Property(e => e.RelevantMaxExperience).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.RelevantMinExperience).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TotalMaxExperience).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TotalMinExperience).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequirementStatusMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requirem__3214EC07F369ACC5");

            entity.ToTable("RequirementStatusMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RequirementStatusCode).HasMaxLength(100);
            entity.Property(e => e.RequirementStatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RequirementTypeMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requirem__3214EC0753EB6895");

            entity.ToTable("RequirementTypeMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RequirementTypeCode).HasMaxLength(100);
            entity.Property(e => e.RequirementTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ResumeFileUploadDetailMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ResumeFi__3214EC070FB9A8A2");

            entity.ToTable("ResumeFileUploadDetailMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrentCtcAnnual)
                .HasColumnType("decimal(38, 17)")
                .HasColumnName("CurrentCTC_Annual");
            entity.Property(e => e.CurrentEmployer).HasMaxLength(100);
            entity.Property(e => e.CurrentLocation).HasMaxLength(100);
            entity.Property(e => e.CurrentPfdeduction).HasColumnName("CurrentPFDeduction");
            entity.Property(e => e.CurrentTakeHomeMonthly)
                .HasColumnType("decimal(38, 17)")
                .HasColumnName("CurrentTakeHome_Monthly");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Email1).HasMaxLength(50);
            entity.Property(e => e.Email2).HasMaxLength(50);
            entity.Property(e => e.ExpectedCtcAnnual)
                .HasColumnType("decimal(38, 17)")
                .HasColumnName("ExpectedCTC_Annual");
            entity.Property(e => e.ExpectedJoinInDays)
                .HasColumnType("decimal(38, 17)")
                .HasColumnName("ExpectedJoinIn_Days");
            entity.Property(e => e.ExpectedPfdeduction).HasColumnName("ExpectedPFDeduction");
            entity.Property(e => e.ExpectedTakeHomeMonthly)
                .HasColumnType("decimal(38, 17)")
                .HasColumnName("ExpectedTakeHome_Monthly");
            entity.Property(e => e.F2favailability).HasColumnName("F2FAvailability");
            entity.Property(e => e.F2finterviewTime)
                .HasColumnType("datetime")
                .HasColumnName("F2FInterviewTime");
            entity.Property(e => e.FileName).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.HighestQualification).HasMaxLength(100);
            entity.Property(e => e.JoinInDate).HasColumnType("date");
            entity.Property(e => e.LastSalaryHike).HasMaxLength(100);
            entity.Property(e => e.Mobile1).HasMaxLength(30);
            entity.Property(e => e.Mobile2).HasMaxLength(30);
            entity.Property(e => e.Mobile3).HasMaxLength(30);
            entity.Property(e => e.NativePlace).HasMaxLength(100);
            entity.Property(e => e.NoticePeriodDays)
                .HasColumnType("decimal(38, 17)")
                .HasColumnName("NoticePeriod_Days");
            entity.Property(e => e.OfferedPackageInLac).HasColumnType("decimal(38, 17)");
            entity.Property(e => e.Pan)
                .HasMaxLength(20)
                .HasColumnName("PAN");
            entity.Property(e => e.RelevantExperienceYear)
                .HasColumnType("decimal(38, 17)")
                .HasColumnName("RelevantExperience_Year");
            entity.Property(e => e.TeleInterviewTime).HasColumnType("datetime");
            entity.Property(e => e.TotalExperienceAnnual)
                .HasColumnType("decimal(38, 17)")
                .HasColumnName("TotalExperience_Annual");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WorkLocation).HasMaxLength(100);
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

        modelBuilder.Entity<SalaryMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalaryMs__3214EC0744E331E9");

            entity.ToTable("SalaryMst");

            entity.Property(e => e.AdditionalHraallowance)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("AdditionalHRAAllowance");
            entity.Property(e => e.Advances).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Basic).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BasicSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ConveyanceAllowance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Ctc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CTC");
            entity.Property(e => e.EmployerContributionToPf)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("EmployerContributionToPF");
            entity.Property(e => e.Esicemployee)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("ESICEmployee");
            entity.Property(e => e.Esicemployer)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("ESICEmployer");
            entity.Property(e => e.FixedConveyanceAllowance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FixedHra)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("FixedHRA");
            entity.Property(e => e.FixedMedicalAllowance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FlexibleAllowance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GrossSalaryPayable).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Hra)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("HRA");
            entity.Property(e => e.Hraallowance)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("HRAAllowance");
            entity.Property(e => e.IncentiveAllowance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncomeTax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MedicalAllowance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NetPayable).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaidLeave).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PayableDays).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Pfemployee)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PFEmployee");
            entity.Property(e => e.Pfemployer)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PFEmployer");
            entity.Property(e => e.ProfessionalTax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalartMonth)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TotalDays).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDeduction).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalEarning).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnpaidLeave).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SalesContactPersonMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalesCon__3214EC07A27E58D0");

            entity.ToTable("SalesContactPersonMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Designation).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Linkedinurl).HasMaxLength(500);
            entity.Property(e => e.Mobile1).HasMaxLength(20);
            entity.Property(e => e.Mobile2).HasMaxLength(20);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SalesLeadActionMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalesLea__3214EC072F502257");

            entity.ToTable("SalesLeadActionMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SalesLeadActionCode).HasMaxLength(50);
            entity.Property(e => e.SalesLeadActionName).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SalesLeadFollowUpMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalesLea__3214EC0743D47331");

            entity.ToTable("SalesLeadFollowUpMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FollowUpDateTime).HasColumnType("datetime");
            entity.Property(e => e.NextFollowUpDateTime).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SalesLeadMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalesLea__3214EC0765DF9F38");

            entity.ToTable("SalesLeadMst");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email1).HasMaxLength(50);
            entity.Property(e => e.Email2).HasMaxLength(50);
            entity.Property(e => e.OrgName).HasMaxLength(100);
            entity.Property(e => e.Phone1).HasMaxLength(20);
            entity.Property(e => e.Phone2).HasMaxLength(20);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.WebsiteUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<SalesLeadStatusMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalesLea__3214EC072DDA280B");

            entity.ToTable("SalesLeadStatusMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SalesLeadStatusCode).HasMaxLength(50);
            entity.Property(e => e.SalesLeadStatusName).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TeamMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TeamMst__3214EC075C655D55");

            entity.ToTable("TeamMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserDetailsMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserDeta__3214EC0701FA4FE6");

            entity.ToTable("UserDetailsMst");

            entity.Property(e => e.AccountNumber).HasMaxLength(500);
            entity.Property(e => e.AdharCardNumber).HasMaxLength(12);
            entity.Property(e => e.BankName).HasMaxLength(500);
            entity.Property(e => e.BloodGroup).HasMaxLength(100);
            entity.Property(e => e.Branch).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnType("datetime");
            entity.Property(e => e.EmergencyContact).HasMaxLength(100);
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.EmployeePersonalEmailId).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(100);
            entity.Property(e => e.IfscCode).HasMaxLength(20);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.OfferDate).HasColumnType("datetime");
            entity.Property(e => e.PancardNumber).HasMaxLength(10);
            entity.Property(e => e.PfaccountNumber).HasMaxLength(30);
            entity.Property(e => e.PostCode).HasMaxLength(100);
            entity.Property(e => e.ProbationPeriod).HasMaxLength(20);
            entity.Property(e => e.Salary).HasColumnType("decimal(38, 18)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserDocumentMst>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserDocu__3214EC0703CE8ACB");

            entity.ToTable("UserDocumentMst");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
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
            entity.Property(e => e.Password).HasMaxLength(100);
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
