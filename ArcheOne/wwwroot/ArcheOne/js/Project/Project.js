﻿$(document).ready(function () {

    $('#ddlResources').select2();
    GetProjectList();
});

var tblProjects = null;

function GetProjectList() {
    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/Project/GetProjectList', null, function (result) {
        if (result.status == true) {

            if (tblProjects !== null) {
                tblProjects.destroy();
                tblProjects = null;
            }

            tblProjects = $('#tblProjects').DataTable({
                "responsive": true,
                "lengthChange": true,
                "paging": true,
                "processing": true,
                "filter": true,
                "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],

                "data": result.data,
                "columns": [
                    {
                        data: null,
                        title: 'Action',
                        render: function (data, type, row) {
                            var actions = '';
                            if (row.isEditable) {
                                actions = '<i class="fa fa-pen pen btn-edit" style="cursor: pointer;" data-toggle="modal" data-target="#modalProject" onclick="GetProjectDetails(' + row.id + ')"></i>';
                            }
                            if (row.isDeletable) {
                                actions = '<i class="fa fa-trash trash btn-delete" style="cursor: pointer;" onclick="ShowDeleteProjectAlert('+row.id+')"></i>';
                            }
                            return actions;
                        }
                    },
                    { data: "id", title: "Id" },
                    { data: "id", title: "Id" },
                    { data: "projectName", title: "Project" },
                    { data: "projectStatus", title: "Project Status" },
                    { data: "resourcesNames", title: "Resources" },
                    {
                        data: null, title: "Created Date", render: function (data, type, row) {
                            var datetime = new Date(row.createdDate);
                            return datetime.toLocaleString('en-US', { day: '2-digit', month: 'short', year: '2-digit', hour: '2-digit', minute: '2-digit' });
                        }
                    }]
            }).buttons().container().appendTo('#tblProjects_wrapper .col-md-6:eq(0)');
        }
        else {
            $.blockUI({
                message: "<h2>" + result.message + "</p>"
            });
        }
        $.unblockUI();
    });
}

function OpenProjectModel() {
    GetResources();
    GetProjectStatus();

}

function GetResources() {
    $.blockUI({ message: "<h2>Please wait</p>" });

    $("#ddlResources").empty();

    ajaxCall("Post", false, '/User/UserListByRoleId', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#ddlResources").append($("<option></option>").val(value.id).html(value.firstName + ' ' + value.lastName));
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

function GetProjectStatus() {
    $.blockUI({ message: "<h2>Please wait</p>" });

    $("#ddlProjectStatus").empty();
    $("#ddlProjectStatus").append($("<option selected value='0'>Select Status</option>"));

    ajaxCall("Post", false, '/Project/GetProjectStatus', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#ddlProjectStatus").append($("<option></option>").val(value.id).html(value.title));
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

function AddUpdateProject() {
    if (validateRequiredFieldsByGroup('modalAddUpdateProject')) {
        $("#btnAddUpdateProject").attr("disabled", true);

        $.blockUI({ message: "<h2>Please wait</p>" });


        var requestModel = {
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
        });
    }
}

function GetProjectDetails(projectId) {
    $("#btnAddUpdateProject").html("Update");
    $("#btnAddUpdateProject").removeClass("btn-success").addClass("btn-warning");

    $.blockUI({ message: "<h2>Please wait</p>" });

    ajaxCall("Post", false, '/Project/GetProjectById?ProjectId=' + projectId, null, function (result) {
        if (result.status == true) {
            $("#projectId").val(projectId);
            $("#txtProjectName").val(result.data.projectName);
            $('#ddlProjectStatus').val(result.data.projectStatus);
            $("#ddlResources").val(result.data.resources);

        }
        else {
            $.blockUI({
                message: "<h2>" + result.message + "</p>"
            });
        }
        $.unblockUI();
    });
}

function CancelProject() {
    $("#projectId").val(0);
    $("#txtProjectName").val('');
    $('#ddlProjectStatus').val(0);
    $("#ddlResources").val(0);

    $("#btnAddUpdateProject").html("Save");
    $("#btnAddUpdateProject").removeClass("btn-warning").addClass("btn-success");

}

function ShowDeleteProjectAlert(projectId)
{
    swal.fire({
        title: "Delete",
        html: "Are you sure you want to Delete this project?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'Cancel',
    })
        .then((result) => {
            console.log(result)
            if (result.isDismissed === false) {
                if (result.isConfirmed) {
                    DeleteProject(projectId);
                }
            } else {
                Toast.fire({ icon: 'warning', title: "Interview status update abort!" });
            }
        });
}

function DeleteProject(projectId) {

    $.blockUI({ message: "<h2>Please wait</p>" });

    ajaxCall("Post", false, '/Project/DeleteProjectById?ProjectId=' + projectId, null, function (result) {
        if (result.status == true) {
            Toast.fire({ icon: 'success', title: result.message });
            setInterval(function () {
                window.location.reload();
            }, 1500)
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}