$(document).ready(function () {

    $('#ddlProject').select2();
    $('#ddlResources').select2();
    GetProjectList();
    GetResources();
});


function GetProjectList() {
    $.blockUI({ message: "<h2>Please wait</p>" });

    $("#ddlProject").empty();
    $("#ddlProject").append($("<option selected value='0'>Select Projects</option>"));

    ajaxCall("Post", false, '/Project/GetAllocatedProjectList', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#ddlProject").append($("<option></option>").val(result.id).html(value.projectName));
            });
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}

function GetResources() {
    $.blockUI({ message: "<h2>Please wait</p>" });

    $("#ddlResources").empty();
    $("#ddlResources").append($("<option selected value='0'>Select Resources</option>"));

    ajaxCall("Post", false, '/User/UserListByRoleId', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                console.log(value);
                $("#ddlResources").append($("<option></option>").val(value.id).html(value.firstName + ' ' + value.lastName));
            })
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}