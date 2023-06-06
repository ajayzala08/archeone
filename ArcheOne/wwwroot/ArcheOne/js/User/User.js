var EditMode = 1;
//var saveData = new FormData();
$(document).ready(function () {
    GetFilteredOrganization();
    $("#btnAddUser").click(function () {
        AddEditUser(0);
    });
});

$('#AddUserPage').click(function () {
    window.location.href = '/User/AddEditUser';
});

function AddEditUser(Id) {
    ajaxCall("Get", false, '/User/AddEditUser?Id=' + Id, null, function (result) {

        $("#AddUserData").html(result.responseText);
        if (Id > 0) {
            $(".preview img").attr('src');
            $(".preview img").show();
        }

    });
}
$("#btnSubmit").click(function () {
    SaveUser();
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
        debugger
        if (validateRequiredFields()) {
            ajaxCallWithoutDataType("Post", false, '/User/SaveUpdateUser', JSON.stringify(saveData), function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/User/UserList");
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
    else {
        Toast.fire({ icon: 'error', title: "Please Select Profile Photo." });
    }
    //var saveData = {
    //    "Id": parseInt($("#txtUserId").val()),
    //    "PhotoUrl": $("#txtPhotoUrl").val(),
    //    "CompanyId": $("#ddlCompany").val(),
    //    "RoleId": $("#ddlRole").val(),
    //    "FirstName": $("#txtFirstName").val(),
    //    "MiddleName": $("#txtMiddleName").val(),
    //    "LastName": $("#txtLastName").val(),
    //    "UserName": $("#txtUserName").val(),
    //    "Password": $("#txtPassword").val(),
    //    "Address": $("#txtAddress").val(),
    //    "Pincode": $("#txtPincode").val(),
    //    "Mobile1": $("#txtMobile1").val(),
    //    "Mobile2": $("#txtMobile2").val(),
    //    "Email": $("#txtEmail").val()
    //}


}

function GetFilteredOrganization() {

    ajaxCall("Get", false, '/User/UserList', null, function (result) {

        $("#divUserList").html(result.responseText);
        ApplyDatatableResponsive('tblUser');

        $(".btn-edit").click(function () {

            EditMode = 1;
            Id = $(this).attr('Id');
            AddEditUser(Id);
        });

        $(".btn-delete").click(function () {
            Id = $(this).attr('Id');
            DeleteUser(Id);
        });
    });
}

function ClearAll() {
    $("#txtUserId").val(''),
        $("#txtPhotoUrl").val(''),
        $("#ddlCompany").val(''),
        $("#ddlRole").val(0),
        $("#txtFirstName").val(0),
        $("#txtMiddleName").val(0),
        $("#txtLastName").val(''),
        $("#txtUserName").val(''),
        $("#txtPassword").val(''),
        $("#txtAddress").val(''),
        $("#txtPincode").val(''),
        $("#txtMobile1").val(''),
        $("#txtMobile2").val(''),
        $("#txtEmail").val('')
}

function SetSelectedFile() {
    debugger;
    // Checking whether FormData is available in browser  
    if (window.FormData !== undefined) {
        var fileData = new FormData();
        var file = $("#txtPhotoUrl").get(0).files[0];

        fileData.append("file", file);
        fileData.append("FirstName", "Full Name");
        fileData.append("RoleId", parseInt("2"));


        $.ajax({
            url: '/User/SaveFile',
            type: "POST",
            //dataType:"json",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            data: fileData,
            success: function (result) {
                debugger
                Toast.fire({ icon: 'error', title: result });
            },
            error: function (err) {
                Toast.fire({ icon: 'error', title: err.statusText });
            }
        });

    } else {
        Toast.fire({ icon: 'error', title: "FormData is not supported." });
    }
}
