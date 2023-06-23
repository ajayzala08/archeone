
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

Insert into PermissionMst values('Dashboard View', 'Dashboard_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Users View', 'Users_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('User Detail View', 'User_Detail_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('User Edit', 'User_Edit', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('User Delete', 'User_Delete', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Teams View', 'Teams_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Holidays View', 'Holidays_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Default Permissions View', 'Default_Permissions_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Default Permissions Edit', 'Default_Permissions_Edit', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('User Permissions View', 'User_Permissions_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('User Permissions Edit', 'User_Permissions_Edit', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Requirements View', 'Requirements_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Policy View', 'Policy_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Leaves View', 'Leaves_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Appraisal View', 'Appraisal_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Leads View', 'Leads_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Daily Tasks View', 'Daily_Tasks_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Projects View', 'Projects_View', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Task Report View', 'Task_Report_View', 1, 0, 0, 0, GETDATE(), GETDATE());

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
			
Insert into dbo.InterviewRoundStatusMst values ('Scheduled', 'Scheduled', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundStatusMst values ('Cleared', 'Cleared', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundStatusMst values ('Rejected', 'Rejected', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundStatusMst values ('No Show', 'No_Show', 1, 0, 0, 0, GETDATE(), GETDATE())
			
Insert into dbo.InterviewRoundTypeMst values ('Skype', 'Skype', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundTypeMst values ('Microsoft Teams', 'Microsoft_Teams', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundTypeMst values ('Zoom', 'Zoom', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundTypeMst values ('Google Meet', 'Google_Meet', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.InterviewRoundTypeMst values ('Call', 'Call', 1, 0, 0, 0, GETDATE(), GETDATE())	
			
Insert into dbo.HireStatusMst values ('To Be Join', 'To_Be_Join', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.HireStatusMst values ('Join', 'Join', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.HireStatusMst values ('No Show', 'No_Show', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.HireStatusMst values ('Bad Delivery', 'Bad_Delivery', 1, 0, 0, 0, GETDATE(), GETDATE())
			
Insert into dbo.OfferStatusMst values ('Cleared', 'Cleared', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.OfferStatusMst values ('Offer', 'Offer', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into dbo.OfferStatusMst values ('Hire', 'Hire', 1, 0, 0, 0, GETDATE(), GETDATE())