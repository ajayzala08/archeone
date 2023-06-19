﻿$(document).ready(function () {
    applyRequiredValidation();
    $("#btnSaveUpdateUserDetails").click(function () {
        SaveUserDetails();
    });
});
function SaveUserDetails() {
    saveData.append("Id", parseInt($("#txtId").val()));
    saveData.append("UserId", parseInt($("#txtUserId").val()));
    saveData.append("EmployeeCode", $("#txtEmployeeCode").val());
    saveData.append("Gender", $("#ddlGender").val());
    saveData.append("EmergencyContact", $("#txtEmergencyContact").val());
    saveData.append("Dob", $("#txtDob").val());
    saveData.append("PostCode", $("#txtPostCode").val());
    saveData.append("EmploymentType", $("#ddlEmploymentType").val());
    saveData.append("Department", $("#ddlDepartment").val());
    saveData.append("Designation", $("#ddlDesignation").val());
    saveData.append("Location", $("#txtLocation").val());
    saveData.append("BloodGroup", $("#txtBloodGroup").val());
    saveData.append("OfferDate", $("#txtOfferDate").val());
    saveData.append("JoinDate", $("#txtJoinDate").val());
    saveData.append("BankName", $("#txtBankName").val());
    saveData.append("AccountNumber", $("#txtAccountNumber").val());
    saveData.append("Branch", $("#txtBranch").val());
    saveData.append("IfscCode", $("#txtIfscCode").val());
    saveData.append("PfaccountNumber", $("#txtPfaccountNumber").val());
    saveData.append("PancardNumber", $("#txtPancardNumber").val());
    saveData.append("AdharCardNumber", $("#txtAdharCardNumber").val());
    saveData.append("Salary", $("#txtSalary").val());
    saveData.append("ReportingManager", $("#ddlReportingManager").val());
    saveData.append("Reason", $("#txtReason").val());
    saveData.append("EmployeePersonalEmailId", $("#txtEmployeePersonalEmailId").val());
    saveData.append("ProbationPeriod", $("#txtProbationPeriod").val());

    console.log(saveData);
    debugger
    if (validateRequiredFields()) {
        ajaxCallWithoutDataType("Post", false, '/UserDetails/SaveUpdateUserDetails', saveData, function (result) {
            console.log(result);
            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                RedirectToPage("/User/User");
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
                $.unblockUI();
            }
        });
    }
    else {
        $.unblockUI();
        Toast.fire({ icon: 'success', title: result.message });
        $("#clearAll").click();
        ClearAll();
    }

}
