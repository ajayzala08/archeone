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
                    {
                        data: null, title: "Actions", render: function (data, type, row) {
                            return '<i class="fa fa-pen pen btn-edit" style="cursor: pointer;" data-toggle="modal" data-target="#modalProject" onclick="GetPermissionDetails(' + row.id + ')"></i>';
                        }
                    },
                    { data: "id", title: "Id" },
                    { data: "permissionName", title: "Permissions" },
                    { data: "permissionRoute", title: "Permission Route(s)" },
                    {
                        data: null,
                        title: 'Status',
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

function GetPermissionDetails(permissionId) {
    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/Permission/GetPermissionDetailsById?PermissionId=' + permissionId, null, function (result) {
        if (result.status == true) {
            $("#permissionId").val(result.data.id);
            $("#txtPermissionTitle").val(result.data.permissionName);
            $("#txtPermissionRoutes").value(result.data.permissionRoute);
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}

function SavePermissionDetails() {
    var permissionRoute = $("#txtPermissionRoutes").value().join(', ');
    var requestModel = {
        "PermissionId": parseInt($("#permissionId").val()),
        "PermissionRoute": permissionRoute
    }

    ajaxCall("Post", false, '/Permission/UpdatePermissionDetails', JSON.stringify(requestModel), function (result) {
        if (result.status == true) {
            Toast.fire({ icon: 'success', title: result.message });
            GetDefaultPermissions($("#slRoles").val());
            $('#modalProject').modal('hide');
            CancelPermissionUpdate();
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}

function CancelPermissionUpdate() {
    $("#permissionId").val(0);
    $("#txtPermissionTitle").val('');
    $("#txtPermissionRoutes").value();
}