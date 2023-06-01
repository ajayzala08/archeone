$(document).ready(function () {
    $("#btnSubmit").click(function () {
        var dataModel = {
            "PhotoUrl": $('#txtPhotoUrl').val(),
            "CompanyId": $('#txtCompanyId').val(),
            "RoleId": $('#txtRoleId').val(),
            "FirstName": $('#txtFirstName').val(),
            "MiddleName": $('#txtMiddleName').val(),
            "LastName": $('#txtLastName').val(),
            "UserName": $('#txtUserName').val(),
            "Password": $('#txtPassword').val(),
            "Address": $('#txtAddress').val(),
            "Pincode": $('#txtPincode').val(),
            "Mobile1": $('#txtMobile1').val(),
            "Mobile2": $('#txtMobile2').val(),
            "Email": $('#txtEmail').val()
        }
        console.log(dataModel);

        if (validateRequiredFields(dataModel)) {
            $.blockUI({
                message: "<h2>Please wait</p>"
            });
            setTimeout($.unblockUI, 5000);
            ajaxCall("Post", false, '/User/User', JSON.stringify(dataModel), function (result) {

                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/User/UserList");
                }
                else {
                    $.blockUI({
                        message: "<h2>Please wait</p>"
                    });
                    setTimeout($.unblockUI, 2000);
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    });
});