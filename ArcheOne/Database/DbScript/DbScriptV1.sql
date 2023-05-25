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


