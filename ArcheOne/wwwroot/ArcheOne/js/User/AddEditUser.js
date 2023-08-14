$(document).ready(function () {


});
function SaveUser() {

    var saveData = new FormData();
    var file = $("#txtPhotoUrl").get(0).files[0];
    saveData.append("Id", parseInt($("#txtUserId").val()));
    saveData.append("PhotoUrl", file);
    saveData.append("CompanyId", $("#ddlCompany").val());
    saveData.append("RoleId", $("#ddlRole").val());
    saveData.append("DepartmentId", $("#ddlDepartment").val());
    saveData.append("DesignationId", $("#ddlDesignation").val());
    saveData.append("FirstName", $("#txtFirstName").val());
    saveData.append("MiddleName", $("#txtMiddleName").val());
    saveData.append("LastName", $("#txtLastName").val());
    saveData.append("UserName", $("#txtUserName").val());
    saveData.append("Password", $("#txtPassword").val());
    saveData.append("Address", $("#txtAddress").val());
    saveData.append("Pincode", $("#txtPincode").val());
    saveData.append("Mobile1", $("#txtMobile1").val());
    saveData.append("Mobile2", $("#txtMobile2").val());
    saveData.append("Email", $("#txtEmail").val());
    saveData.append("IsActive", false);
  
    if (validateRequiredFields()) {
        $.blockUI();
        ajaxCallWithoutDataType("Post", false, '/User/SaveUpdateUser', saveData, function (result) {
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
    }
}

$("#btnCloseUser").click(function () {
    RedirectToPage("/User/User");
});

$("#btnSaveUpdateUser").click(function () {
    applyRequiredValidation();

    SaveUser();

});

$("#ddlRole").change(function () {
    GetDesignationByRoleAndDepartment();
});

$("#ddlDepartment").change(function () {
    GetDesignationByRoleAndDepartment();
});

function GetDesignationByRoleAndDepartment() {
    var roleId = $("#ddlRole").val() ?? 0;
    var departmentId = $("#ddlDepartment").val() ?? 0;

    $.blockUI();


    $("#ddlDesignation").empty();
    $("#ddlDesignation").append($("<option selected value='0'>Select Designation</option>"));
    ajaxCallWithoutDataType("Post", false, '/User/GetDesignationByRoleAndDepartment?RoleId=' + roleId + '&DepartmentId=' + departmentId, null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#ddlDesignation").append($("<option></option>").val(value.id).html(value.designation));
            });
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}
