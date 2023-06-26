$(document).ready(function () {
    applyRequiredValidation();
    $("#btnSaveUpdateUser").click(function () {
        
        SaveUser();
    });
});
function SaveUser() {
    if (window.FormData !== undefined) {
        $.blockUI();
        var saveData = new FormData();
        var file = $("#txtPhotoUrl").get(0).files[0];
        saveData.append("Id", parseInt($("#txtUserId").val()));
        saveData.append("PhotoUrl", file);
        saveData.append("CompanyId", $("#ddlCompany").val());
        saveData.append("RoleId", $("#ddlRole").val());
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

        console.log(saveData);
        if (validateRequiredFields()) {
            ajaxCallWithoutDataType("Post", false, '/User/SaveUpdateUser', saveData, function (result) {
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
    else {
        Toast.fire({ icon: 'error', title: "Please Select Profile Photo." });
    }
}

                                                                                            