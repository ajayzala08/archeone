----------------------------------------Added by NP on 25-05-23-----------------------------------Start--------
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'CompanyMst')
BEGIN 
	Create table dbo.CompanyMst(
			Id int identity(1,1) primary key,
			CompanyName nvarchar(100) not null,
			[Address] nvarchar(500) not null,
			Pincode nvarchar(10) not null,
			Phone nvarchar(20) not null,
			Mobile1 nvarchar(20) not null,
			Mobile2 nvarchar(20) not null,
			Email nvarchar(50) not null,
			Website nvarchar(100) not null,
			LogoURL nvarchar(max) not null,
			IsActive bit default(1) not null,
			IsDelete bit default(0) not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'CompanyMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'CompanyMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'UserMst')
BEGIN 
	Create table dbo.UserMst(
			Id int identity(1,1) primary key,
			CompanyId int not null,
			FirstName nvarchar(100) not null,
			MiddleName nvarchar(100) not null,
			LastName nvarchar(100) not null,
			UserName nvarchar(50) not null,
			[Password] nvarchar(20) not null,
			[Address] nvarchar(500) not null,
			Pincode nvarchar(10) not null,
			Mobile1 nvarchar(20) not null,
			Mobile2 nvarchar(20) not null,
			Email nvarchar(50) not null,
			PhotoURL nvarchar(max) not null,
			IsActive bit default(1) not null,
			IsDelete bit default(0) not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'UserMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'UserMst Table Already Exist' 
END

insert into dbo.CompanyMst values('Arche Softronix','Vadodara','390019','0265-446687','9878990654','7896546733',N'Arche@gmail.com',
'www.Archesoftronix.com','logo.png',1,0,0,0,getdate(),getdate())

insert into dbo.usermst values(1,'Admin','','Super','S_Admin','12345','Vadodara','390015','8878964532','9976536725','SuperAdmin@gmail.com','photoURL',1,0,0,0,getdate(),getdate())

----------------------------------------Added by NP on 25-05-23-----------------------------------End--------
---------------------------------------Executed on Local Server on 25-05-23------------------by NP-----------

----------------------------------------Added by PP on 29-05-23-----------------------------------Start--------
--------------------------------LinkMst-------------------------------------
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'LinkMst')
BEGIN 
	Create table dbo.LinkMst(
			Id int identity(1,1) primary key,
			UserId int not null,
			ResetPasswordLink nvarchar(max) not null,
			IsClicked bit not null,
			CreatedDate datetime not null,
			ExpiredDate datetime not null,
			);
	PRINT 'Table Created' 
END
ELSE
BEGIN 
	PRINT 'Table Already Exist' 
END
GO
----------------------------------------Added by PP on 29-05-23-----------------------------------End--------
---------------------------------------Executed on Local Server on 29-05-23------------------by PP-----------

----------------------------------------Added by NP on 29-05-23-----------------------------------Start--------
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'RoleMst')
BEGIN 
	Create table dbo.RoleMst(
			Id int identity(1,1) primary key,
			RoleName nvarchar(100) not null,
			RoleCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'RoleMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'RoleMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'PermissionMst')
BEGIN 
	Create table dbo.PermissionMst(
			Id int identity(1,1) primary key,
			PermissionName nvarchar(100) not null,
			PermissionCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'PermissionMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'PermissionMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'DefaultPermissions')
BEGIN 
	Create table dbo.DefaultPermissions(
			Id int identity(1,1) primary key,
			RoleId int not null,
			PermissionId int not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'DefaultPermissions Table Created' 
END
ELSE
BEGIN 
	PRINT 'DefaultPermissions Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'UserPermissions')
BEGIN 
	Create table dbo.UserPermissions(
			Id int identity(1,1) primary key,
			UserId int not null,
			PermissionId int not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'UserPermissions Table Created' 
END
ELSE
BEGIN 
	PRINT 'UserPermissions Table Already Exist' 
END
----------------------------------------Added by NP on 29-05-23-----------------------------------End--------
---------------------------------------Executed on Local Server on 29-05-23------------------by NP-----------

----------------------------------------Added by DS on 01-06-23-----------------------------------Start--------

Alter table UserMst add RoleId int

----------------------------------------Added by DS on 01-06-23-----------------------------------End--------
---------------------------------------Executed on Local Server on 01-06-23------------------by DS-----------

----------------------------------------Added by TS on 11-06-23-----------------------------------Start--------
Insert into dbo.RoleMst values ('SuperAdmin', 'SA001', 1, 0, 0, 0, GETDATE(), GETDATE());

----------------------------------------Added by TS on 01-06-23-----------------------------------End--------
---------------------------------------Executed on Local Server on 01-06-23------------------by TS-----------

----------------------------------------Added by NP on 05-06-23-----------------------------------Start--------

--drop table dbo.RequirementMst
--drop table dbo.PositionTypeMst
--drop table dbo.RequirementTypeMst
--drop table dbo.RequirementForMst
--drop table dbo.EmploymentTypeMst
--drop table dbo.InterviewStatusMst
--drop table dbo.InterviewTypeStatusMst
--drop table dbo.HireStatusMst
--drop table dbo.OfferStatusMst
--drop table dbo.ClientMst
--drop table dbo.ResumeFileUploadDetailMst
--drop table dbo.ResumeFileUploadHistoryMst
--drop table dbo.ScheduleInterviewMst
--drop table dbo.CandidateMst
--drop table dbo.HireMst
--drop table dbo.OfferMst
--drop table dbo.TeamMst

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'RequirementForMst')
BEGIN 
	Create table dbo.RequirementForMst(
			Id int identity(1,1) primary key,
			RequirementForName nvarchar(100) not null,
			RequirementForCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'RequirementForMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'RequirementForMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'ClientMst')
BEGIN 
	Create table dbo.ClientMst(
			Id int identity(1,1) primary key,
			ClientName nvarchar(100) not null,
			ClientCode nvarchar(100) not null,
			CompanyId int not null,
			EmailId nvarchar(100) not null,
			MobileNo nvarchar(20) not null,
			POCNamePrimary nvarchar(100),
			POCNumberPrimary nvarchar(20),
			POCEmailPrimary nvarchar(100),
			POCNameSecondary nvarchar(100),
			POCNumberSecondary nvarchar(20),
			POCEmailSecondary nvarchar(100),
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'ClientMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'ClientMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'PositionTypeMst')
BEGIN 
	Create table dbo.PositionTypeMst(
			Id int identity(1,1) primary key,
			PositionTypeName nvarchar(100) not null,
			PositionTypeCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'PositionTypeMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'PositionTypeMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'RequirementTypeMst')
BEGIN 
	Create table dbo.RequirementTypeMst(
			Id int identity(1,1) primary key,
			RequirementTypeName nvarchar(100) not null,
			RequirementTypeCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'RequirementTypeMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'RequirementTypeMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'EmploymentTypeMst')
BEGIN 
	Create table dbo.EmploymentTypeMst(
			Id int identity(1,1) primary key,
			EmploymentTypeName nvarchar(100) not null,
			EmploymentTypeCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'EmploymentTypeMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'EmploymentTypeMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'RequirementStatusMst')
BEGIN 
	Create table dbo.RequirementStatusMst(
			Id int identity(1,1) primary key,
			RequirementStatusName nvarchar(100) not null,
			RequirementStatusCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'RequirementStatusMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'RequirementStatusMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'RequirementMst')
BEGIN 
	Create table dbo.RequirementMst(
			Id int identity(1,1) primary key,
			RequirementForId int not null,
			ClientId int not null,
			JobCode nvarchar(100) not null,
			MainSkill nvarchar(max) null,
			NoOfPosition int not null,
			[Location] nvarchar(Max) not null,
			EndClient nvarchar(100) not null,
			TotalMinExperience decimal(18,0) not null,
			TotalMaxExperience decimal(18,0) not null,
			RelevantMinExperience decimal(18,0) not null,
			RelevantMaxExperience decimal(18,0) not null,
			ClientBillRate decimal(18,0) not null,
			CandidatePayRate decimal(18,0) not null,
			PositionTypeId int not null,
			RequirementTypeId int not null,
			EmploymentTypeId int not null,
			POCName nvarchar(100) not null,
			MandatorySkills nvarchar(max) not null,
			JobDescription nvarchar(Max) null,
			AssignedUserIds nvarchar(100) null,
			RequirementStatusId int not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'RequirementMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'RequirementMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'ResumeFileUploadMst')
BEGIN 
	Create table dbo.ResumeFileUploadMst(
			Id int identity(1,1) primary key,
			RequirementId int not null,
			FileName nvarchar(500) not null,
			FileExtension nvarchar(20) not null,
			FileSize nvarchar(50) not null,
			FilePath nvarchar(max) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'ResumeFileUploadMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'ResumeFileUploadMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'ResumeFileUploadDetailMst')
BEGIN 
	Create table dbo.ResumeFileUploadDetailMst(
			Id int identity(1,1) primary key,
			ResumeFileUploadId int not null,
			Mobile1 nvarchar(30) not null,
			Mobile2 nvarchar(30) null,
			Mobile3 nvarchar(30) null,
			Email1 nvarchar(50) not null,
			Email2 nvarchar(50) null,
			TotalExperience_Annual decimal(18,0) not null,
			RelevantExperience_Year decimal(18,0) not null,
			HighestQualification nvarchar(100) not null,
			GapReason nvarchar(max) null,
			CurrentEmployer nvarchar(100) null,
			CurrentDesignation nvarchar(max) null,
			CurrentCTC_Annual decimal(18,0) not null,
			CurrentTakeHome_Monthly decimal(18,0) not null,
			CurrentPFDeduction bit not null,
			ExpectedCTC_Annual decimal(18,0) not null,
			ExpectedTakeHome_Monthly decimal(18,0) not null,
			ExpectedPFDeduction bit not null,
			LastSalaryHike nvarchar(100) null,
			SalaryHikeReason nvarchar(max) null,
			NoticePeriod_Days decimal(18,0) not null,
			ExpectedJoinIn_Days decimal(18,0) not null,
			ReasonForEarlyJoin nvarchar(max) null,
			OfferInHand bit not null,
			OfferInHandReason nvarchar(max) null,
			HasAllDocuments bit not null,
			CurrentLocation nvarchar(100) not null,
			WorkLocation nvarchar(100) not null,
			ReasonForRelocation nvarchar(max) null,
			NativePlace nvarchar(100) not null,
			DOB datetime not null,
			PAN nvarchar(20) not null,
			TeleInterviewTime datetime null,
			F2FAvailability bit not null,
			F2FInterviewTime datetime null,
			Skills nvarchar(max) null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'ResumeFileUploadDetailMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'ResumeFileUploadDetailMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'InterviewRoundStatusMst')
BEGIN 
	Create table dbo.InterviewRoundStatusMst(
			Id int identity(1,1) primary key,
			InterviewRoundStatusName nvarchar(100) not null,
			InterviewRoundStatusCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'InterviewRoundStatusMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'InterviewRoundStatusMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'InterviewRoundTypeMst')
BEGIN 
	Create table dbo.InterviewRoundTypeMst(
			Id int identity(1,1) primary key,
			InterviewRoundTypeName nvarchar(100) not null,
			InterviewRoundTypeCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'InterviewRoundTypeMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'InterviewRoundTypeMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'HireStatusMst')
BEGIN 
	Create table dbo.HireStatusMst(
			Id int identity(1,1) primary key,
			HireStatusName nvarchar(100) not null,
			HireStatusCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'HireStatusMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'HireStatusMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'OfferStatusMst')
BEGIN 
	Create table dbo.OfferStatusMst(
			Id int identity(1,1) primary key,
			OfferStatusName nvarchar(100) not null,
			OfferStatusCode nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'OfferStatusMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'OfferStatusMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'InterviewMst')
BEGIN 
	Create table dbo.InterviewMst(
			Id int identity(1,1) primary key,
			ResumeFileUploadId int not null,
			ResumeFileUploadDetailId int not null,
			HireStatusId int not null,
			OfferStatusId int not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'InterviewMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'InterviewMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'InterviewRoundMst')
BEGIN 
	Create table dbo.InterviewRoundMst(
			Id int identity(1,1) primary key,
			InterviewId int not null,
			InterviewRoundStatusId int not null,
			InterviewRoundTypeId int not null,
			InterviewStartDateTime datetime not null,
			InterviewEndDateTime datetime not null,
			InterviewByUserId int not null,
			InterviewLocation nvarchar(100) not null,
			Notes nvarchar(max) null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'InterviewRoundMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'InterviewRoundMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'CandidateMst')
BEGIN 
	Create table dbo.CandidateMst(
			Id int identity(1,1) primary key,
			InterviewId int not null,
			FirstName nvarchar(100) not null,
			MiddleName nvarchar(100) not null,
			LastName nvarchar(100) not null,
			Gender nvarchar(20) not null,
			DOB DateTime not null,
			MaritalStatus nvarchar(20) not null,
			Mobile1 nvarchar(20) not null,
			Mobile2 nvarchar(20) null,
			Email1 nvarchar(50) not null,
			Email2 nvarchar(50) not null,
			AadharNumber nvarchar(50) null,
			PanNumber nvarchar(50) null,
			CurrentAddress nvarchar(500) null,
			PermanentAddress nvarchar(500) null,
			CountryId int not null,
			StateId int not null,
			CityId int not null,
			EmergencyContact nvarchar(20) null,
			EndClient nvarchar(100) null,
			JoiningLocation nvarchar(500) not null,
			CurrentDesignation nvarchar(50) not null,
			OfferDesignation nvarchar(50) not null,
			TotalExperience decimal(18,0) not null,
			RelevantExperience decimal(18,0) not null,
			Skill nvarchar(max) not null,
			SelectionDate DateTime not null,
			OfferDate DateTime not null,
			JoiningDate DateTime not null,
			CTC decimal(18,0) not null,
			ECTC decimal(18,0) not null,
			MarginPercentage decimal(18,0) not null,
			GP decimal(18,0) not null,
			ClientBillRate int null,
			CandidatePayRate int null,
			EmploymentTypeId int not null,
			HireStatusId int not null,
			BankAccountNo nvarchar(100) null,
			BankName nvarchar(100) null,
			BankBranch nvarchar(100) null,
			IFSCCode nvarchar(100) null,
			Note nvarchar(Max) null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'CandidateMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'CandidateMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'TeamMst')
BEGIN 
	Create table dbo.TeamMst(
			Id int identity(1,1) primary key,
			TeamLeadId int not null,
			TeamMemberId int not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'TeamMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'TeamMst Table Already Exist' 
END

----------------------------------------Added by NP on 05-06-23-----------------------------------END--------
---------------------------------------Executed on Local Server on 06-06-23------------------by NP-----------

----------------------------------------Added by PP on 06-06-23-----------------------------------START--------
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'SalesLeadMst')
BEGIN 
	Create table dbo.SalesLeadMst(
			Id int identity(1,1) primary key,
			OrgName nvarchar(100) not null,
			CountryId int not null,
			StateId int not null,
			CityId  int not null,
			Address nvarchar(500) not null,
			Phone1 nvarchar(20) not null,
			Phone2 nvarchar(20) not null,
			Email1 nvarchar(50) not null,
			Email2 nvarchar(50) not null,
			WebsiteUrl nvarchar(500) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'SalesLeadMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'SalesLeadMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'SalesContactPersonMst')
BEGIN 
	Create table dbo.SalesContactPersonMst(
			Id int identity(1,1) primary key,
			SalesLeadId int not null,
			FirstName nvarchar(50) not null,
			LastName nvarchar(50) not null,
			Email nvarchar(50) not null,
			Designation  nvarchar(100) null,
			Mobile1 nvarchar(20) not null,
			Mobile2 nvarchar(20) null,
			Linkedinurl nvarchar(500) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'SalesContactPersonMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'SalesContactPersonMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'SalesLeadStatusMst')
BEGIN 
	Create table dbo.SalesLeadStatusMst(
			Id int identity(1,1) primary key,
			SalesLeadStatusName nvarchar(50) not null,
			SalesLeadStatusCode nvarchar(50) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'SalesLeadStatusMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'SalesLeadStatusMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'SalesLeadActionMst')
BEGIN 
	Create table dbo.SalesLeadActionMst(
			Id int identity(1,1) primary key,
			SalesLeadActionName nvarchar(50) not null,
			SalesLeadActionCode nvarchar(50) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'SalesLeadActionMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'SalesLeadActionMst Table Already Exist' 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'SalesLeadFollowUpMst')
BEGIN 
	Create table dbo.SalesLeadFollowUpMst(
			Id int identity(1,1) primary key,
			SalesLeadId int not null,
			SalesContactPersonId int not null,
			FollowUpDateTime  datetime not null,
			NextFollowUpDateTime datetime not null,
			SalesLeadStatusId int not null,
			SalesLeadActionId int not null,
			Notes nvarchar(max) null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null
			);
	PRINT 'SalesLeadFollowUpMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'SalesLeadFollowUpMst Table Already Exist' 
END
----------------------------------------Added by PP on 06-06-23-----------------------------------END--------
---------------------------------------Executed on Local Server on 06-06-23------------------by PP-----------