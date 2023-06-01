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
            $.blockUI({
                message: "<h2>" + result.message + "</p>"
            });
        }
        $.unblockUI();
    });
}

function GetDefaultPermissions(RoleId) {
    ajaxCall("Post", false, '/Permission/GetDefaultPermissionList?RoleId='+RoleId, null, function (result) {
        if (result.status == true) {

            $("#btnUpdatePermission").removeAttr("disabled");

            columnNames = Object.keys(result.data[0]);

            $('#tbDefaultPermissions').DataTable({
                "responsive": true,
                "lengthChange": true,
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
                            return '<input type="checkbox" value="' + row.id + '">';
                        }
                    }]
            });
        }
        else {

            $("#btnUpdatePermission").attr("disabled",true);

            $.blockUI({
                message: "<h2>" + result.message + "</p>"
            });
        }
        $.unblockUI();
    });
}