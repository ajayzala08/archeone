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

var dataTable = null;

function GetDefaultPermissions(RoleId) {
    ajaxCall("Post", false, '/Permission/GetDefaultPermissionList?RoleId=' + RoleId, null, function (result) {
        if (result.status == true) {

            $("#btnUpdatePermission").removeAttr("disabled");

            if (dataTable !== null) {
                dataTable.destroy();
                dataTable = null;
            }

            dataTable = $('#tbDefaultPermissions').DataTable({
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

            $.blockUI({
                message: "<h2>" + result.message + "</p>"
            });
        }
        $.unblockUI();
    });
}

function UpdateDefaultPermission() {
    var Data = [];

    $('.permissionBox:checked').each(function () {
        Data.push(parseInt($(this).val()));
    });

    var updateDefaultPermissionReqModel = {
        "RoleId": parseInt($("#slRoles").val()),
        "CreatedBy": 1,
        "PermissionIds": Data
    }

    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/Permission/UpdateDefaultPermission', JSON.stringify(updateDefaultPermissionReqModel), function (result) {
        if (result.status == true) {
            Toast.fire({ icon: 'success', title: result.message });
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}