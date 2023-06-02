$(document).ready(function () {
    $("#btnSubmit").click(function () {
        debugger
        var dataModel = {
            "PhotoUrl": $('#txtPhotoUrl').val(),
            /*"CompanyId": $('#txtCompanyId').val(),*/
            "CompanyId": $('#ddlCompany option:selected').val(),
            "RoleId": $('#ddlRole option:selected').val(),
            //"RoleId": $('#txtRoleId').val(),
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
        if (validateRequiredFields()) {
            console.log("method");
            ajaxCall("Post", false, '/User/AddUser', JSON.stringify(dataModel), function (result) {

                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    $("#btnClose").click();
                    ClearAll();
                    GetFilteredOrganization();
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    });
});