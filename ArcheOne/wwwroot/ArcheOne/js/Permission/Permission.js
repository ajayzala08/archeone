$(document).ready(function () {
    GetRoleList();
});


function GetRoleList() {
    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/Role/RoleList', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#slRoles").append($("<option></option>").val(data.id).html(value.roleName));
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

function GetDefaultPermissions() {
    $('#example1').DataTable(
        {
            "columnDefs": [
                { "searchable": true, "orderable": true, targets: "_all" },
                { "className": "text-center custom-middle-align", targets: "_all" }
            ],
            "processing": true,
            "serverSide": true,
            "ajax":
            {
                "url": "/Role/RoleList",
                "type": "POST",
                "dataType": "JSON",
                "dataSrc": function (json) {
                    // Settings.  
                    jsonObj = $.parseJSON(json.data)

                    // Data  
                    return jsonObj.data;
                }
            },
            "columns": [
                { "data": "Name", "autowidth": true },
                { "data": "Surname", "autowidth": true },
                { "data": "Office", "autowidth": true },
                { "data": "Email", "autowidth": true },
                { "data": "Telephone", "autowidth": true },
                { "data": "Cellphone", "autowidth": true },
                { "data": "CreatedDate", "autowidth": true },
                { "data": "CreatedBy", "autowidth": true },
                { "data": "UpdatedDate", "autowidth": true },
                { "data": "UpdatedBy", "autowidth": true }

            ]
        });
}