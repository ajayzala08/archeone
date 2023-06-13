
truncate table dbo.CompanyMst
truncate table dbo.RoleMst
truncate table dbo.UserMst
truncate table dbo.PermissionMst
truncate table dbo.UserPermissions
truncate table dbo.DefaultPermissions

truncate table dbo.RequirementForMst
truncate table dbo.PositionTypeMst
truncate table dbo.RequirementTypeMst
truncate table dbo.EmploymentTypeMst
truncate table dbo.RequirementStatusMst
truncate table dbo.InterviewRoundStatusMst
truncate table dbo.InterviewRoundTypeMst
truncate table dbo.HireStatusMst
truncate table dbo.OfferStatusMst


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


------------------------------------SetUp Constant Masters----------------------------
Insert into dbo.RequirementForMst values ('In House', 'In_House', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.RequirementForMst values ('For Client', 'For_Client', 1, 0, 0, 0, GETDATE(), GETDATE())

Insert into dbo.PositionTypeMst values ('IT', 'IT', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.PositionTypeMst values ('NON-IT', 'NON_IT', 1, 0, 0, 0, GETDATE(), GETDATE())
			
Insert into dbo.RequirementTypeMst values ('Drive', 'Drive', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.RequirementTypeMst values ('In Progress', 'In_Progress', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.RequirementTypeMst values ('Exclusive', 'Exclusive', 1, 0, 0, 0, GETDATE(), GETDATE())
			
Insert into dbo.EmploymentTypeMst values ('Contractual', 'Contractual', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.EmploymentTypeMst values ('Permenant', 'Permenant', 1, 0, 0, 0, GETDATE(), GETDATE())
			
Insert into dbo.RequirementStatusMst values ('Active', 'Active', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.RequirementStatusMst values ('On Hold', 'On_Hold', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.RequirementStatusMst values ('In-Active', 'In_Active', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.RequirementStatusMst values ('Closed', 'Closed', 1, 0, 0, 0, GETDATE(), GETDATE())
			
Insert into dbo.InterviewRoundStatusMst values ('Set', 'Set', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundStatusMst values ('Interview', 'Interview', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundStatusMst values ('Re-Schedule', 'Re_Schedule', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundStatusMst values ('No Show', 'No_Show', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundStatusMst values ('Offer', 'Offer', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundStatusMst values ('Rejected', 'Rejected', 1, 0, 0, 0, GETDATE(), GETDATE())
			
Insert into dbo.InterviewRoundTypeMst values ('Face To Face', 'FaceToFace', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundTypeMst values ('Skype', 'Skype', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundTypeMst values ('Microsoft Teams', 'Microsoft_Teams', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundTypeMst values ('Zoom', 'Zoom', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundTypeMst values ('Google Meet', 'Google_Meet', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundTypeMst values ('Call', 'Call', 1, 0, 0, 0, GETDATE(), GETDATE())	
			
Insert into dbo.HireStatusMst values ('To Be Join', 'To_Be_Join', 1, 0, 0, 0, GETDATE(), GETDATE())	
Insert into dbo.HireStatusMst values ('Join', 'Join', 1, 0, 0, 0, GETDATE(), GETDATE())	
Insert into dbo.HireStatusMst values ('No Show[BD]', 'No_Show', 1, 0, 0, 0, GETDATE(), GETDATE())	
			
Insert into dbo.OfferStatusMst values ('Offer', 'Offer', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.OfferStatusMst values ('Hire', 'Hire', 1, 0, 0, 0, GETDATE(), GETDATE())