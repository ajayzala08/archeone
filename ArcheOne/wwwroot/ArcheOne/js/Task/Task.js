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
    GetTaskList();
});


var tblDailyTask = null;

function GetTaskList() {
    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/Task/GetTaskList', null, function (result) {
        if (result.status == true) {

            if (tblDailyTask !== null) {
                tblDailyTask.destroy();
                tblDailyTask = null;
            }

            var showUserName = false;

            if (result.data.length > 0) {
                showUserName = result.data[0].showUserName;
            }

            tblDailyTask = $('#tblDailyTask').DataTable({
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
                                actions = '<i class="fa fa-pen pen btn-edit" style="cursor: pointer;" data-toggle="modal" data-target="#modalDailyTask" onclick="GetTaskDetails(' + row.id + ')"></i> | ';
                            }
                            actions += '<i class="fa fa-trash trash btn-delete" style="cursor: pointer;" onclick="ShowDeleteTaskAlert(' + row.id + ')"></i>';
                            return actions;
                        }
                    },
                    //{ data: "id", title: "Id" },

                    { data: "createdByName", title: "Resource", visible: showUserName, searchable: showUserName },
                    { data: "projectName", title: "Project" },
                    {
                        data: null, title: "Task Date", render: function (data, type, row) {
                            var datetime = new Date(row.taskDate);
                            return datetime.toLocaleString('en-US', { day: '2-digit', month: 'short', year: '2-digit' });
                        }
                    },
                    {
                        data: null, title: "Entered Date", render: function (data, type, row) {
                            var datetime = new Date(row.createdDate);
                            return datetime.toLocaleString('en-US', { day: '2-digit', month: 'short', year: '2-digit' });
                        }
                    },
                    { data: "taskModule", title: "Module" },
                    { data: "taskDescription", title: "Description" },
                    { data: "taskStatus", title: "Task Status" },
                    { data: "timeSpent", title: "Time Spent" }]
            }).buttons().container().appendTo('#tblDailyTask_wrapper .col-md-6:eq(0)');
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}

function GetProjectList() {

    return new Promise((resolve, reject) => {
        $.blockUI({ message: "<h2>Please wait</p>" });

        $("#ddlProject").empty();
        $("#ddlProject").append($("<option selected value='0'>Select Projects</option>"));

        $("#ddlProjectTask").empty();
        $("#ddlProjectTask").append($("<option selected value='0'>Select Projects</option>"));

        ajaxCall("Post", false, '/Project/GetAllocatedProjectList', null, function (result) {
            if (result.status == true) {
                $.each(result.data, function (data, value) {
                    $("#ddlProject").append($("<option></option>").val(value.id).html(value.projectName));
                    $("#ddlProjectTask").append($("<option></option>").val(value.id).html(value.projectName));
                });
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

function GetResources() {
    return new Promise((resolve, reject) => {
        $.blockUI({ message: "<h2>Please wait</p>" });

        $("#ddlResources").empty();
        $("#ddlResources").append($("<option selected value='0'>Select Resources</option>"));

        ajaxCall("Post", false, '/User/UserListByRoleId', null, function (result) {
            if (result.status == true) {
                $.each(result.data, function (data, value) {
                    $("#ddlResources").append($("<option></option>").val(value.id).html(value.firstName + ' ' + value.lastName));
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

    $("#sddlTimeSpentMM").text("Please Select Time Spent (Minutes)");
    if (validateRequiredFieldsByGroup('modalAddUpdateDailyTask')) {

        var timeSpentHH = $("#ddlTimeSpentHH option:selected").text();
        var timeSpentMM = $("#ddlTimeSpentMM option:selected").text();

        if (timeSpentHH == "00" && timeSpentMM == "00") {
            $("#" + $("#ddlTimeSpentMM").attr("errorspan")).removeClass('d-none');
            $("#" + $("#ddlTimeSpentMM").attr("divcontainer")).addClass('has-error');
            $("#" + $("#ddlTimeSpentMM").attr("id")).addClass('is-invalid');
            $("#sddlTimeSpentMM").text("Please Select Minimum 5 Minutes.");

        } else {

            $("#btnAddUpdateDailyTask").attr("disabled", true);

            $.blockUI({ message: "<h2>Please wait</p>" });
            var requestModel = {
                "Id": parseInt($("#taskId").val()),
                "ProjectId": $("#ddlProjectTask").val(),
                "TaskModule": $("#txtModule").val(),
                "TaskStatus": $("#ddlDailyTaskStatus option:selected").text(),
                "TaskDate": $("#txtTaskDate").val(),
                "TimeSpentHH": $("#ddlTimeSpentHH option:selected").text(),
                "TimeSpentMM": $("#ddlTimeSpentMM option:selected").text(),
                "TaskDescription": $("#txtTaskDescription").val(),
            }

            ajaxCall("Post", false, '/Task/AddUpdateTask', JSON.stringify(requestModel), function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    setInterval(function () {
                        window.location.reload();
                    }, 1500)
                } else {
                    Toast.fire({ icon: 'error', title: result.message });
                }
                $.unblockUI();

                $("#btnAddUpdateTask").removeAttr("disabled");
            });
        }
    }
}

function GetTaskDetails(taskId) {
    $("#btnAddUpdateDailyTask").html("Update");
    $("#btnAddUpdateDailyTask").removeClass("btn-success").addClass("btn-warning");

    OpenTaskModel().then(() => {
        $.blockUI({ message: "<h2>Please wait</p>" });

        ajaxCall("Post", false, '/Task/GetTaskById?TaskId=' + taskId, null, function (result) {
            if (result.status == true) {
                $("#taskId").val(taskId);
                $("#ddlProjectTask").val(result.data.projectId).trigger('change');;
                $("#txtModule").val(result.data.taskModule);
                $("#ddlDailyTaskStatus").val(result.data.taskStatus).trigger('change');;
                $("#txtTaskDate").val(result.data.taskDate.split('T')[0]);
                $("#ddlTimeSpentHH option:contains(" + result.data.timeSpent.split(':')[0] + ")").prop('selected', true).trigger('change');;
                $("#ddlTimeSpentMM option:contains(" + result.data.timeSpent.split(':')[1] + ")").prop('selected', true).trigger('change');;
                $("#txtTaskDescription").val(result.data.taskDescription);

            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
            }
            $.unblockUI();
        });
    }).catch(error => {
        console.error(error);
    });
}

function OpenTaskModel() {
    return new Promise((resolve, reject) => {
        GetProjectList()
            .then(() => {
                return GetProjectStatus();
            })
            .then(() => {
                resolve(); // Resolve the promise when all methods complete
            })
            .catch(error => {
                reject(error); // Reject the promise if any error occurs
            });
    });
}

function CancelDailyTask() {
    $("#taskId").val(0);
    $("#ddlProjectTask").val(0).trigger('change');;
    $("#txtModule").val('');
    $("#ddlDailyTaskStatus").val(0).trigger('change');;
    $("#txtTaskDate").val('');
    $("#ddlTimeSpentHH").val(0).trigger('change');;
    $("#ddlTimeSpentMM").val(0).trigger('change');;
    $("#txtTaskDescription").val('');

    $("#btnAddUpdateDailyTask").html("Save");
    $("#btnAddUpdateDailyTask").removeClass("btn-warning").addClass("btn-success");
}

function ShowDeleteTaskAlert(taskId) {
    swal.fire({
        title: "Delete",
        html: "Are you sure you want to Delete this Task?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'Cancel',
    })
        .then((result) => {
            console.log(result)
            if (result.isDismissed === false) {
                if (result.isConfirmed) {
                    DeleteTask(taskId);
                }
            } else {
                Toast.fire({ icon: 'warning', title: "Task delete abort!" });
            }
        });
}

function DeleteTask(taskId) {

    $.blockUI({ message: "<h2>Please wait</p>" });

    ajaxCall("Post", false, '/Task/DeleteTaskById?TaskId=' + taskId, null, function (result) {
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