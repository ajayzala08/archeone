$(document).ready(function () {
    GetRoleList();
});


function GetRoleList() {
    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/Role/RoleList', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#slRoles").append($("<option></option>").val(value.id).html(value.roleName));
            })
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}

function GetUserList(RoleId) {

    $.blockUI({ message: "<h2>Please wait</p>" });

    $("#slUsers").empty();
    $("#slUsers").append($("<option selected disabled value='0'>Select User</option>"));

    if (dataTable !== null) {
        dataTable.clear().draw();
    }

    ajaxCall("Post", false, '/User/UserListByRoleId?RoleId=' + RoleId, null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#slUsers").append($("<option></option>").val(value.id).html(value.firstName + " " + value.middleName + " " + value.lastName));
            })
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}

var dataTable = null;

function GetUserPermissions(UserId) {
    ajaxCall("Post", false, '/Permission/GetUserPermissions?UserId=' + UserId, null, function (result) {
        if (result.status == true) {

            $("#btnUpdatePermission").removeAttr("disabled");

            if (dataTable !== null) {
                dataTable.destroy();
                dataTable = null;
            }

            dataTable = $('#tbUserPermissions').DataTable({
                "responsive": true,
                "lengthChange": false,
                "paging": false,
                "processing": true, // for show progress bar
                "filter": true, // this is for disable filter (search box)

                "data": result.data,
                "columns": [
                    { data: "id", title: "Id" },
                    { data: "permissionName", title: "Permissions" },
                    {
                        data: null,
                        title: 'Action',
                        render: function (data, type, row) {
                            if (row.isDefaultPermission) {
                                return '<input type="checkbox" class="permissionBox" checked value="' + row.id + '">';
                            } else {
                                return '<input type="checkbox" class="permissionBox" value="' + row.id + '">';
                            }
                        }
                    }]
            });
        }
        else {

            $("#btnUpdatePermission").attr("disabled", true);

            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}

function UpdateUserPermission() {
    var Data = [];

    $('.permissionBox:checked').each(function () {
        Data.push(parseInt($(this).val()));
    });

    var requestModel = {
        "UserId": parseInt($("#slUsers").val()),
        "CreatedBy": 1,
        "PermissionIds": Data
    }

    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/Permission/UpdateUserPermission', JSON.stringify(requestModel), function (result) {
        if (result.status == true) {
            Toast.fire({ icon: 'success', title: result.message });
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}