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
        debugger
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

function ShowPermissionAlert() {

    var updateRoleWithUsers = false;

    swal.fire({
        title: "Update Permissions",
        html: "Do you want to update permissions for existing users with this Role?<br><b><p3> Or </p3></b><br>Or update permissions for future users with this Role?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: 'Update for Existing Users',
        cancelButtonText: 'Update for Future Users',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33'
    })
        .then((result) => {
            console.log(result)
            if (result.isDismissed === false) {
                if (result.isConfirmed) {
                    updateRoleWithUsers = true;
                } else {
                    updateRoleWithUsers = false;
                }
                UpdateDefaultPermission(updateRoleWithUsers)
            } else {
                Toast.fire({ icon: 'warning', title: "Permissions update abort!" });
            }
        });
}

function UpdateDefaultPermission(UpdateRoleWithUsers) {

    var Data = [];
    $('.permissionBox:checked').each(function () {
        Data.push(parseInt($(this).val()));
    });

    var requestModel = {
        "RoleId": parseInt($("#slRoles").val()),
        "CreatedBy": 1,
        "UpdateRoleWithUsers": UpdateRoleWithUsers,
        "PermissionIds": Data
    }

    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/Permission/UpdateDefaultPermission', JSON.stringify(requestModel), function (result) {
        if (result.status == true) {
            Toast.fire({ icon: 'success', title: result.message });
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}