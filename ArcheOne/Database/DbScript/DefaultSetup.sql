
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

Insert into RoleMst values ('Super Admin', 'Super_Admin', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into RoleMst values ('Admin', 'Admin', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into RoleMst values ('Manager', 'Manager', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into RoleMst values ('Team Lead', 'Team_Lead', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into RoleMst values ('Professional', 'Professional', 1, 0, 0, 0, GETDATE(), GETDATE());

------------------------------------SetUp Departments----------------------------

Insert into DepartmentMst values ('Administration', 'Administration', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMst values ('Research and Development', 'Research_And_Development', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMst values ('System Administration', 'System_Administration', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMst values ('Software Development', 'Software_Development', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMst values ('Quality Analyst', 'Quality_Analyst', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMst values ('Human Resource', 'Human_Resource', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMst values ('Marketing', 'Marketing', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMst values ('Designer', 'Designer', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMst values ('Content', 'Content', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMst values ('Finance', 'Finance', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMst values ('Sales', 'Sales', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DepartmentMSt values ('Recruitment', 'Recruitment', 1, 0, 0, 0, GETDATE(), GETDATE())

------------------------------------SetUp Designation----------------------------

Insert into DesignationMst values ('Director', 2, 1, 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Vice Director', 2, 1, 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Senior Manager', 3, '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Associate Manager', 3, '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Junior Manager', 3, '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Senior Team Lead', 4, '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Associate Team Lead', 4, '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Junior Team Lead', 4, '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Senior Software Developer', 5, 4, 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Associate Software Developer', 5, 4, 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Junior Software Development', 5, 4, 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Business Developer', 5, 11, 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Accountant', 5, 10, 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('System Engineer', 5, 3, 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into DesignationMst values ('Technical Writer', 5, 9, 1, 0, 0, 0, GETDATE(), GETDATE());

--------------------------------------SetUp Permissions-----------------------------

Insert into PermissionMst values('Dashboard View', 'Dashboard_View', 'Dashboard/Index', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Users View', 'Users_View', 'User/User, User/UserList', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('User Detail View', 'User_Detail_View', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('User Add', 'User_Edit', 'User/AddEditUser', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into PermissionMst values('User Edit', 'User_Edit', 'User/AddEditUser', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('User Delete', 'User_Delete', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Teams View', 'Teams_View', 'Team/Team, Team/TeamList', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Holidays View', 'Holidays_View', 'Holiday/Holiday, Holiday/HolidayList', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Default Permissions View', 'Default_Permissions_View', 'Permission/DefaultPermission, Permission/GetDefaultPermissionList, Role/RoleList', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Default Permissions Edit', 'Default_Permissions_Edit', 'Permission/UpdateDefaultPermission', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('User Permissions View', 'User_Permissions_View', 'Permission/UserPermission, Role/RoleList, User/UserListByRoleId, Permission/GetUserPermissions', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('User Permissions Edit', 'User_Permissions_Edit', 'Permission/UpdateUserPermission', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Requirements View', 'Requirements_View', 'Requirement/Index', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Requirements Add', 'Requirements_Add', 'Requirement/AddEditRequirement', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Requirements Edit', 'Requirements_Edit', 'Requirement/AddEditRequirement', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Requirements Delete', 'Requirements_Delete', 'Requirement/DeleteRequirement', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Uploaded Resume View', 'Uploaded_Resume_View', 'UploadedResume/UploadedResume, UploadedResume/GetUploadedResumeList', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Policy View', 'Policy_View', 'Policy/Policy Policy/PolicyList', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Leaves View', 'Leaves_View', 'Leaves/Leaves', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Appraisal View', 'Appraisal_View', 'Appraisal/Appraisal, Appraisal/AppraisalList', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Leads View', 'Leads_View', 'SalesLead/Index', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Daily Tasks View', 'Daily_Tasks_View', 'Task/Task, Project/GetAllocatedProjectList, Task/GetTaskList, Project/GetProjectStatus, User/UserListByRoleId', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Projects View', 'Projects_View', 'Project/Project, Project/GetProjectList', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Task Report View', 'Task_Report_View', 'Task/TaskReport', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Appraisal Add View', 'Appraisal_Add_View', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Appraisal Edit View', 'Appraisal_Edit_View', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Appraisal Delete View', 'Appraisal_Delete_View', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Appraisal Rating Add View', 'Appraisal_Rating_Add_View', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Appraisal Rating Edit View', 'Appraisal_Rating_Edit_View', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Leave Add View', 'Leave_Add_View', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Leave Edit View', 'Leave_Edit_View', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Leave Delete View', 'Leave_Delete_View', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Leave Status Change View', 'Leave_Status_Change_View', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst values('Event View','Event_View','', 1, 0, 1, 1, GETDATE(), GETDATE())
Insert into PermissionMst values('Event Add View','Event_Add_View','', 1, 0, 1, 1, GETDATE(), GETDATE())
Insert into PermissionMst values('Event Edit View','Event_Edit_View','', 1, 0, 1, 1, GETDATE(), GETDATE())
Insert into PermissionMst values('Event Delete View','Event_Delete_View','', 1, 0, 1, 1, GETDATE(), GETDATE())
Insert into PermissionMst values('Salary View','Salary_View','', 1, 0, 1, 1, GETDATE(), GETDATE())
Insert into PermissionMst values('Upload Salary Sheet View','Upload_Salary_Sheet_View','', 1, 0, 1, 1, GETDATE(), GETDATE())
Insert into PermissionMst values('Uploaded Resume Status Update', 'Uploaded_Resume_Status_Update', '', 1, 0, 0, 0, GETDATE(), GETDATE());
Insert into PermissionMst Values ('Holidays Add View', 'Holidays_Add_View', '', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into PermissionMst Values ('Holidays Edit View', 'Holidays_Edit_View', '', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into PermissionMst Values ('Holidays Delete View', 'Holidays_Delete_View', '', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into PermissionMst Values ('Policy Add View', 'Policy_Add_View', '', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into PermissionMst Values ('Policy Edit View', 'Policy_Edit_View', '', 1, 0, 0, 0, GETDATE(), GETDATE())
Insert into PermissionMst Values ('Policy Delete View', 'Policy_Delete_View', '', 1, 0, 0, 0, GETDATE(), GETDATE())


------------------------------------SetUp SuperAdmin User----------------------------
insert into dbo.UserMst values(1, 1, 0, 0,'Admin','','Super','S_Admin','1lo5OLz+GwC4ocaqa2Nt+Q==','Vadodara','390015','8878964532','9976536725','SuperAdmin@gmail.com','photoURL',1,0,0,0,getdate(),getdate())


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

Insert into CalenderYearMst values('2020','2020',0,1,0,1,1,GETDATE(),GETDATE())
Insert into CalenderYearMst values('2021','2021',0,1,0,1,1,GETDATE(),GETDATE())
Insert into CalenderYearMst values('2022','2022',0,1,0,1,1,GETDATE(),GETDATE())
Insert into CalenderYearMst values('2023','2023',1,1,0,1,1,GETDATE(),GETDATE())

Insert into LeaveTypeMst values('SickLeave',6,1,0,1,0,1,1,GETDATE(),GETDATE())
Insert into LeaveTypeMst values('CasualLeave',6,1,0,1,0,1,1,GETDATE(),GETDATE())
Insert into LeaveTypeMst values('EarnedLeave',6,1,0,1,0,1,1,GETDATE(),GETDATE())

Insert into LeaveTypeMst values('SickLeave',6,2,0,1,0,1,1,GETDATE(),GETDATE())
Insert into LeaveTypeMst values('CasualLeave',6,2,0,1,0,1,1,GETDATE(),GETDATE())
Insert into LeaveTypeMst values('EarnedLeave',6,2,0,1,0,1,1,GETDATE(),GETDATE())

Insert into LeaveTypeMst values('SickLeave',6,3,0,1,0,1,1,GETDATE(),GETDATE())
Insert into LeaveTypeMst values('CasualLeave',6,3,0,1,0,1,1,GETDATE(),GETDATE())
Insert into LeaveTypeMst values('EarnedLeave',6,3,0,1,0,1,1,GETDATE(),GETDATE())

Insert into LeaveTypeMst values('SickLeave',6,4,1,1,0,1,1,GETDATE(),GETDATE())
Insert into LeaveTypeMst values('CasualLeave',6,4,1,1,0,1,1,GETDATE(),GETDATE())
Insert into LeaveTypeMst values('EarnedLeave',6,4,1,1,0,1,1,GETDATE(),GETDATE())

Insert into LeaveStatusMst values('Approve',1,0,1,1,GETDATE(),GETDATE())
Insert into LeaveStatusMst values('Reject',1,0,1,1,GETDATE(),GETDATE())
Insert into LeaveStatusMst values('Pending',1,0,1,1,GETDATE(),GETDATE())