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

----------------------------------------Added by SP on 01-06-23-----------------------------------Start--------

---------------------------------------------------RequirementMst------------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'RequirementMst')
BEGIN 
	Create table dbo.RequirementMst(
			Id int identity(1,1) primary key,
			RecruitmentForId int not null,
			ClientId int not null,
			JobCode nvarchar(50) not null,
			MainSkill nvarchar(50)  null,
			NoofPosition int not null,
			Location nvarchar(50) not null,
			EndClient nvarchar(50) not null,
			TotalMinExperience int not null,
			TotalMaxExperience int not null,
			RelevantMinExperience int not null,
			RelevantMaxExperience int not null,
			BillRate decimal not null,
			PayRate decimal not null,
			PositionTypeId int not null,
			RecruitmentTypeId int not null,
			EmploymentTypeId int not null,
			POCName nvarchar(100) not null,
			MandatorySkills nvarchar(100) not null,
			JobDescription nvarchar(Max) not null,
			AssignToid int  null,
			Status int null,
			IsActive bit default(1) not null,
			IsDelete bit default(0) not null,
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


---------------------------------------------------PositionTypeMst------------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'PositionTypeMst')
BEGIN 
	Create table dbo.PositionTypeMst(
			Id int identity(1,1) primary key,
			PositionTypeName nvarchar(100) not null,
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
-------------------------------------------------RequirementTypeMst------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'RequirementTypeMst')
BEGIN 
	Create table dbo.RequirementTypeMst(
			Id int identity(1,1) primary key,
			RequirementTypeName nvarchar(100) not null,
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

--------------------------------------------RequirementForMst----------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'RequirementForMst')
BEGIN 
	Create table dbo.RequirementForMst(
			Id int identity(1,1) primary key,
			RequirementForName nvarchar(100) not null,
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
--------------------------------------EmploymentTypeMst-----------------------------------------------


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'EmploymentTypeMst')
BEGIN 
	Create table dbo.EmploymentTypeMst(
			Id int identity(1,1) primary key,
			EmploymentTypeName nvarchar(100) not null,
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

------------------------------------------------InterviewStatusMst------------------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'InterviewStatusMst')
BEGIN 
	Create table dbo.InterviewStatusMst(
			Id int identity(1,1) primary key,
			InterviewStatusName nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'InterviewStatusMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'InterviewStatusMst Table Already Exist' 
END

-------------------------------------------InterviewTypeStatusMst-----------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'InterviewTypeStatusMst')
BEGIN 
	Create table dbo.InterviewTypeStatusMst(
			Id int identity(1,1) primary key,
			InterviewStatusName nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'InterviewTypeStatusMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'InterviewTypeStatusMst Table Already Exist' 
END

-----------------------------------------HireStatusMst--------------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'HireStatusMst')
BEGIN 
	Create table dbo.HireStatusMst(
			Id int identity(1,1) primary key,
			HireStatusName nvarchar(100) not null,
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

--------------------------------------------OfferStatusMst-------------------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'OfferStatusMst')
BEGIN 
	Create table dbo.OfferStatusMst(
			Id int identity(1,1) primary key,
			OfferStatusName nvarchar(100) not null,
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

----------------------------------------ClientMst-----------------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'ClientMst')
BEGIN 
	Create table dbo.ClientMst(
			Id int identity(1,1) primary key,
			ClientName nvarchar(100) not null,
			EmailId nvarchar(100) not null,
			MobileNo nvarchar(20) not null,
			POCNamePrimary nvarchar(100)  null,
			POCNumberPrimary nvarchar(20)  null,
			POCEmailIdPrimary nvarchar(100)  null,
			POCNameSecondary nvarchar(100)  null,
			POCNumberSecondary nvarchar(20)  null,
			POCEmailIdSecondary nvarchar(100)  null,
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


-------------------------------------ResumeFileUploadDetailMst-------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'ResumeFileUploadDetailMst')
BEGIN 
	Create table dbo.ResumeFileUploadDetailMst(
			Id int identity(1,1) primary key,
			RecruitmentJobId int not null,
			ApplicantName nvarchar(100) not null,
			ApplicantDOB DateTime not null,
			Address nvarchar(100) not null,
			Count int null,
			AlterCount int null,
			Email nvarchar(50) not null,
			TotalExperience int not null,
			RelevantExperience int not null,
			CurrentCompany nvarchar(100) not null,
			CurrentDesignation nvarchar(100) not null,
			NoticePeriod int null,
			CanJoin bit null,
			CTC decimal not null,
			ECTC decimal not null,
			Reason nvarchar(max) not null,
			AnyInterviewOffer bit null,
			Education nvarchar(max) not null,
			CurrentLocation nvarchar(100) not null,
			PrefferedLocation nvarchar(100) not null,
			Native nvarchar(100) not null,
			ResumeName nvarchar(50) not null,
			ResumeStatusId int not null,
			Skills nvarchar(Max) not null,
			FamilyCount int not null,
			FriendCount int not null,
			ReasonGap nvarchar(100) not null,
			CurrentTakeHome decimal not null,
			CurrentDrawing decimal null,
			LastSalaryHike Datetime not null,
			ExpectedTakeHome decimal not null,
			ExpectedDrawing decimal null,
			HikeReason nvarchar(100) not null,
			HowJoinEarlyReason nvarchar(100) not null,
			ReasonForJoin nvarchar(100) not null,
			HaveDocs bit null,
			ReasonOfRelocation nvarchar(100) null,
			PanNumber nvarchar(100) null,
			TeliPhonicInTime nvarchar(20) null,
			F2FAvaillability bit null,
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

--------------------------------------------ResumeFileUploadHistoryMst-------------------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'ResumeFileUploadHistoryMst')
BEGIN 
	Create table dbo.ResumeFileUploadHistoryMst(
			Id int identity(1,1) primary key,
			FileName nvarchar(100) not null,
			FileExtension nvarchar(50) not null,
			FileSize int not null,
			FilePath nvarchar(100) not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'ResumeFileUploadHistoryMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'ResumeFileUploadHistoryMst Table Already Exist' 
END


------------------------------------ScheduleInterviewMst--------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'ScheduleInterviewMst')
BEGIN 
	Create table dbo.ScheduleInterviewMst(
			Id int identity(1,1) primary key,
			ResumeFileUploadId int not null,
			RequirementId int not null,
			InterviewStatusId int not null,
			InterviewTypeStatusId int not null,
			InterviewDate DateTime not null,
			CandidateName nvarchar(100) not null,
			InterviewBy nvarchar(100) not null,
			InterviewLocation nvarchar(100) not null,
			Note nvarchar(50) not null,
			StatusId int not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'ScheduleInterviewMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'ScheduleInterviewMst Table Already Exist' 
END

-----------------------------------------CandidateMst-------------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'CandidateMst')
BEGIN 
	Create table dbo.CandidateMst(
			Id int identity(1,1) primary key,
			CandidateName nvarchar(100) not null,
			Gender nvarchar(20) not null,
			DOB DateTime not null,
			MaritalStatus nvarchar(20) not null,
			ContactNumber nvarchar(20) not null,
			AlternateNumber nvarchar(20) null,
			Email nvarchar(50) not null,
			AadharNumber nvarchar(50) null,
			PanNumber nvarchar(50) null,
			CurrentAddress nvarchar(100) null,
			PermanentAddress nvarchar(100) null,
			Country int null,
			EmergencyContact nvarchar(20) null,
			CompanyName nvarchar(100) null,
			JoiningLocation nvarchar(100)not null,
			CurrentDesignation nvarchar(20)not null,
			OfferDesignation nvarchar(20)not null,
			TotalExperience int null,
			RelevantExperience int null,
			Skill nvarchar(100) not null,
			SelectionDate DateTime  null,
			OfferDate DateTime null,
			JoiningDate DateTime null,
			CTC int not null,
			ECTC int not null,
			MarginPercentage int null,
			GP int null,
			EmploymentTypeId int null,
			InterviewStatusId int null,
			BillRate int null,
			PayRate int null,
			BankAccountNo nvarchar(100) null,
			BanlName nvarchar(100) null,
			Branch nvarchar(100) null,
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

--------------------------------------HireMst------------------------------------------------


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'HireMst')
BEGIN 
	Create table dbo.HireMst(
			Id int identity(1,1) primary key,
			RequirementId int not null,
			ResumeId int not null,
			ScheduleInterviewId int not null,
			HireStatusId int not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'HireMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'HireMst Table Already Exist' 
END

---------------------------------OfferMst---------------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  
			TABLE_NAME = 'OfferMst')
BEGIN 
	Create table dbo.OfferMst(
			Id int identity(1,1) primary key,
			RequirementId int not null,
			ResumeId int not null,
			ScheduleInterviewId int not null,
			OfferStatusId int not null,
			IsActive bit not null,
			IsDelete bit not null,
			CreatedBy int not null,
			UpdatedBy int not null,
			CreatedDate datetime not null,
			UpdatedDate datetime not null,
			);
	PRINT 'OfferMst Table Created' 
END
ELSE
BEGIN 
	PRINT 'OfferMst Table Already Exist' 
END



---------------------------------------Executed on Local Server on 01-06-23------------------by SP-----------


----------------------------------------Added by SP on 05-06-23-----------------------------------Start--------

---------------------------------------------------TeamMst------------------------------------------


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

---------------------------------------Executed on Local Server on 05-06-23------------------by SP-----------