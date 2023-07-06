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

    $('.card-header').append('<input type="hidden" id="showUserName" value="false">');

    GetTaskList()
        .then(() => {
            // This is the event for removing the column from "Column Vibility" of datatable.
            // This should be used with Promise only because once table is rendered then only this event can be attach to it.
            if ($('#showUserName').val() == "false") {
                $(".buttons-colvis[aria-controls='tblDailyTask']").on("click", function () {
                    $('[aria-controls="tblDailyTask"][data-cv-idx="2"]').remove();
                });
            }
        })
        .catch(error => {
            console.log(error);
        });
});


var tblDailyTask = null;

function GetTaskList() {

    return new Promise((resolve, reject) => {
        var showUserName = false;

        if (tblDailyTask !== null) {
            if (!$.fn.DataTable.isDataTable(tblDailyTask)) {
                tblDailyTask = $('#tblDailyTask').DataTable();
            }
            tblDailyTask.destroy();
            tblDailyTask = null;
        }

        tblDailyTask = $('#tblDailyTask').DataTable({
            ajax: {
                url: "/Task/GetTaskList",
                type: "POST",
                data: function (requestModel) {
                    requestModel.ProjectId = parseInt($("#ddlProject").val());
                    requestModel.ResourceId = parseInt($("#ddlResources").val() || 0);


                    if ($("#txtFromDate").val() != "") {
                        requestModel.FromDate = $("#txtFromDate").val();
                    }

                    if ($("#txtToDate").val() != "") {
                        requestModel.ToDate = $("#txtToDate").val();
                    }

                    return requestModel;
                },
            },
            "drawCallback": function (response) {
                if (response.json.status == true) {
                    if (response.json.data != null && response.json.data.length > 0) {
                        showUserName = response.json.data[0].showUserName;
                        showUserName ? $("#dddlResources").show() : $("#dddlResources").hide().remove();
                        $('#showUserName').val(showUserName);
                        $('#lblTotalTime').text(response.json.calculatedTime);
                    } else {
                        $('#lblTotalTime').text("00:00");
                    }

                    this.api().column(2).visible(showUserName);
                    this.api().column(2).search(showUserName);

                    resolve();
                } else {

                    Toast.fire({ icon: 'error', title: response.json.message });
                    reject();
                }
            },
            responsive: true,
            //dom: 'lfBrtip',
            dom: "<'row'<'col-sm col-md'l><'col-sm col-md'f>>" +
                "<'row dom_wrapper fh-fixedHeader'B>" +
                "<'row'<'col-sm col-md'tr>>" +
                "<'row'<'col-sm col-md'i><'col-sm col-md'p>>",
            buttons: ["copy", "csv", "excel", "pdf", "print", "colvis"],
            serverSide: true,
            columns: [{
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
            { data: "id", title: "Id", name: "Id", visible: false, searchable: false },
            { data: "createdByName", title: "Resource", name: "CreatedByName", visible: showUserName, searchable: showUserName },
            { data: "projectName", title: "Project", name: "ProjectName" },
            {
                data: null, title: "Task Date", name: "TaskDate", render: function (data, type, row) {
                    var datetime = new Date(row.taskDate);
                    return datetime.toLocaleString('en-US', { day: '2-digit', month: 'short', year: '2-digit' });
                }
            },
            {
                data: null, title: "Due Date", name: "DueDate", render: function (data, type, row) {
                    if (row.dueDate != null && row.dueDate != "") {
                        var datetime = new Date(row.dueDate);
                        return datetime.toLocaleString('en-US', { day: '2-digit', month: 'short', year: '2-digit' });
                    } else return "";
                }
            },
            { data: "taskModule", title: "Module/User Story", name: "TaskModule" },
            { data: "taskName", title: "Task Name", name: "TaskName" },
            { data: "taskDescription", title: "Description", name: "TaskDescription" },
            { data: "taskStatus", title: "Task Status", name: "TaskStatus" },
            { data: "timeSpent", title: "Time Spent", name: "TimeSpent" }],
        });
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

function ToggleDueDate() {
    if ($('#ddlDailyTaskStatus option:selected').text() == "InProgress") {
        $('#dtxtTaskDueDate').parent().show();
        $('#txtTaskDueDate').attr("isRequired", "1");
    } else {
        $('#dtxtTaskDueDate').parent().hide();
        $('#txtTaskDueDate').removeAttr("isRequired");
    }
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
                "TaskName": $("#txtTaskName").val(),
            }

            var dueDate = $("#txtTaskDueDate").val();
            if (dueDate != null && dueDate != "") {
                requestModel.DueDate = dueDate;
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

                $("#btnAddUpdateDailyTask").removeAttr("disabled");
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
                $("#ddlDailyTaskStatus option:contains(" + result.data.taskStatus + ")").prop('selected', true).trigger('change');
                $("#txtTaskDate").val(result.data.taskDate.split('T')[0]);
                $("#ddlTimeSpentHH option:contains(" + result.data.timeSpent.split(':')[0] + ")").prop('selected', true).trigger('change');
                $("#ddlTimeSpentMM option:contains(" + result.data.timeSpent.split(':')[1] + ")").prop('selected', true).trigger('change');
                $("#txtTaskDescription").val(result.data.taskDescription);
                $("#txtTaskDueDate").val(result.data.dueDate != null ? result.data.dueDate.split('T')[0] : null);
                $("#txtTaskName").val(result.data.taskName);

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
    $("#txtTaskDueDate").val('');
    $("#txtTaskName").val('');

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

function GenerateExcelReport() {
    ajaxCall("Post", false, '/Task/GenerateTaskReport', null, function (result) {
        if (result.status == true) {
            Toast.fire({ icon: 'success', title: result.message });
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}