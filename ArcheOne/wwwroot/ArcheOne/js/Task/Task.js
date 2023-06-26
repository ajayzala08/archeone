$(document).ready(function () {

    $('#ddlProject').select2();
    $('#ddlProjectTask').select2();
    $('#ddlResources').select2();
    $('#ddlDailyTaskStatus').select2();

    $('#ddlTimeSpentHH').select2();
    $('#ddlTimeSpentMM').select2();

    GetProjectList();
    GetResources();
    GetProjectStatus();
});

function GetProjectList() {
    $.blockUI({ message: "<h2>Please wait</p>" });

    $("#ddlProject").empty();
    $("#ddlProject").append($("<option selected value='0'>Select Projects</option>"));

    $("#ddlProjectTask").empty();
    $("#ddlProjectTask").append($("<option selected value='0'>Select Projects</option>"));

    ajaxCall("Post", false, '/Project/GetAllocatedProjectList', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#ddlProject").append($("<option></option>").val(result.id).html(value.projectName));
                $("#ddlProjectTask").append($("<option></option>").val(result.id).html(value.projectName));
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
                $("#ddlResources").append($("<option></option>").val(value.id).html(value.firstName + ' ' + value.lastName));
            })
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}

function GetProjectStatus() {
    return new Promise((resolve, reject) => {
        $.blockUI({ message: "<h2>Please wait</p>" });

        $("#ddlDailyTaskStatus").empty();
        $("#ddlDailyTaskStatus").append($("<option selected value='0'>Select Status</option>"));

        ajaxCall("Post", false, '/Project/GetProjectStatus', null, function (result) {
            if (result.status == true) {
                $.each(result.data, function (data, value) {
                    $("#ddlDailyTaskStatus").append($("<option></option>").val(value.id).html(value.title));
                })
                resolve();
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
                reject();
            }
            $.unblockUI();
        });
    });
}

function AddUpdateDailyTask() {
    if (validateRequiredFieldsByGroup('modalAddUpdateDailyTask')) {
        var timeSpentHH = $("#ddlTimeSpentMM").val();
        var timeSpentMM = $("#ddlTimeSpentMM").val();
        if (timeSpentHH == "00" && timeSpentMM == "00") {

        }
        $("#btnAddUpdateDailyTask").attr("disabled", true);

        $.blockUI({ message: "<h2>Please wait</p>" });


        /*var requestModel = {
            "Id": parseInt($("#projectId").val()),
            "ProjectName": $("#txtProjectName").val(),
            "ProjectStatus": $('#ddlProjectStatus option:selected').text(),
            "Resources": $("#ddlResources").val().join(',')
        }

        ajaxCall("Post", false, '/Project/AddUpdateProject', JSON.stringify(requestModel), function (result) {
            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                setInterval(function () {
                    window.location.reload();
                }, 1500)
            } else {
                Toast.fire({ icon: 'error', title: result.message });
            }
            $.unblockUI();

            $("#btnAddUpdateProject").removeAttr("disabled");
        });*/
    }
}