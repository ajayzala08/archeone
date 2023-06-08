var EditMode = 1;
//var saveData = new FormData();
$(document).ready(function () {
    GetFilteredUserList();

    $("#btnAddUser").click(function () {
        AddEditUser(0);
    });
});

$('#AddUserPage').click(function () {
    window.location.href = '/User/AddEditUser';
});

function AddEditUser(Id) {
    debugger
    ajaxCall("Get", false, '/User/AddEditUser?Id=' + Id, null, function (result) {
        if (Id > 0) {
            RedirectToPage('/User/AddEditUser?Id=' + Id)
            $(".preview img").attr('src');
            $(".preview img").show();
        }
        else {
            RedirectToPage("/User/AddEditUser")
        }
    });
}

function GetFilteredUserList() {
    ajaxCall("Get", false, '/User/UserList', null, function (result) {
        $("#divUserList").html(result.responseText);
        ApplyDatatableResponsive('tblUser');

        $(".btn-edit").click(function () {
            debugger
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
