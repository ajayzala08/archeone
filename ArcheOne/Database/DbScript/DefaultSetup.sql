
truncate table dbo.CompanyMst
truncate table dbo.RoleMst
truncate table dbo.UserMst
truncate table dbo.PermissionMst
truncate table dbo.UserPermissions
truncate table dbo.DefaultPermissions


------------------------------------SetUp Company----------------------------
insert into dbo.CompanyMst values('Arche Softronix','Vadodara','390019','0265-446687','9878990654','7896546733',N'Arche@gmail.com',
'www.Archesoftronix.com','logo.png',1,0,0,0,getdate(),getdate())

------------------------------------SetUp Roles----------------------------
Insert into dbo.RoleMst values ('Super Admin', 'Super_Admin', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into dbo.RoleMst values ('Admin', 'Admin', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into dbo.RoleMst values ('Project Manager', 'Project_Manager', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into dbo.RoleMst values ('Project Team Lead', 'Project_Team_Lead', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into dbo.RoleMst values ('Developer', 'Developer', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into dbo.RoleMst values ('Sales Team Lead', 'Sales_Team_Lead', 1, 0, 0, 0, GETDATE(), GETDATE());

--------------------------------------SetUp Permissions-----------------------------
insert into dbo.PermissionMst values('Dashboard View','Dashboard_View',1,0,0,0,getdate(),getdate())
insert into dbo.PermissionMst values('Default Permission View','Default_Permission_View',1,0,0,0,getdate(),getdate())
insert into dbo.PermissionMst values('Default Permission Update','Default_Permission_Update',1,0,0,0,getdate(),getdate())
insert into dbo.PermissionMst values('User Permission View','User_Permission_View',1,0,0,0,getdate(),getdate())
insert into dbo.PermissionMst values('User Permission Update','User_Permission_Update',1,0,0,0,getdate(),getdate())
insert into dbo.PermissionMst values('User View','User_View',1,0,0,0,getdate(),getdate())
insert into dbo.PermissionMst values('User Add','User_Add',1,0,0,0,getdate(),getdate())
insert into dbo.PermissionMst values('User Update','User_Update',1,0,0,0,getdate(),getdate())
insert into dbo.PermissionMst values('User Delete','User_Delete',1,0,0,0,getdate(),getdate())

------------------------------------SetUp SuperAdmin User----------------------------
insert into dbo.UserMst values(1,'Admin','','Super','S_Admin','123456','Vadodara','390015','8878964532','9976536725','SuperAdmin@gmail.com','photoURL',1,0,0,0,getdate(),getdate(),1)