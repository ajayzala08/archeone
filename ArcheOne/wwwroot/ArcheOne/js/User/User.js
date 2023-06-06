var EditMode = 1;
$(document).ready(function () {
    GetFilteredOrganization();
    $("#btnAddUser").click(function () {
        AddEditUser(0);
    });
});

function AddEditUser(Id) {
    ajaxCall("Get", false, '/User/AddEditUser?Id=' + Id, null, function (result) {
        $('#AddUserModelPopup').modal('show');
        $("#AddUserModelData").html(result.responseText);
        if (Id > 0) {
            $(".preview img").attr('src');
            $(".preview img").show();
        }
        $("#btnSubmit").click(function () {
            SaveUser();
        });


        $("#ddlCountry").change(function () {

            var CountryId = parseInt($(this).val());
            var StateId = parseInt($("#txtSelectedStateId").val());

            LoadStateByCountry(CountryId, StateId);
        });

        $("#ddlState").change(function () {
            var StateId = parseInt($(this).val());
            var CityId = parseInt($("#txtSelectedCityId").val());
            LoadCityByState(StateId, CityId)
        });

        $("#ddlCountry").change();

    });
}

function SaveUser() {
    var saveData = {
        "Id": parseInt($("#txtUserId").val()),
        "PhotoUrl": $("#txtPhotoUrl").val(),
        "CompanyId": $("#ddlCompany").val(),
        "RoleId": $("#ddlRole").val(),
        "FirstName": $("#txtFirstName").val(),
        "MiddleName": $("#txtMiddleName").val(),
        "LastName": $("#txtLastName").val(),
        "UserName": $("#txtUserName").val(),
        "Password": $("#txtPassword").val(),
        "Address": $("#txtAddress").val(),
        "Pincode": $("#txtPincode").val(),
        "Mobile1": $("#txtMobile1").val(),
        "Mobile2": $("#txtMobile2").val(),
        "Email": $("#txtEmail").val()
    }
    console.log(saveData);
    debugger
    if (validateRequiredFields()) {
        ajaxCall("Post", false, '/User/SaveUpdateUser', JSON.stringify(saveData), function (result) {

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

