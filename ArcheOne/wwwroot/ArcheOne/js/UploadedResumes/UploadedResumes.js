$(document).ready(function () {
    GetUploadedResumes(1);

    $("#dRemoteLocation").hide();
    $('input[type=radio][name=rdInterviewMedium]').change(function () {
        if (this.value == 'WalkIn') {
            $("#dRemoteLocation").hide();
            $("#dWalkIn").show();
        }
        else if (this.value == 'RemoteLocation') {
            GetInterviewRoundTypeList();
            $("#dRemoteLocation").show();
            $("#dWalkIn").hide();
        }
    });

    $('#modal-default').on('hidden.bs.modal', function () {

        document.documentElement.style.overflow = 'hidden';
        document.body.style.pointerEvents = 'none';
        // Reset the form fields
        location.reload();
    });

    $("#btnCancelReschedule").hide();
});

var tbUploadedResumes = null;
var tbScheduleInterviews = null;

function GetUploadedResumes(ResumeFileUploadId) {

    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/UploadedResume/GetUploadedResumeList?ResumeFileUploadId=' + ResumeFileUploadId, null, function (result) {
        if (result.status == true) {

            if (tbUploadedResumes !== null) {
                tbUploadedResumes.destroy();
                tbUploadedResumes = null;
            }

            tbUploadedResumes = $('#tbUploadedResumes').DataTable({
                "responsive": true,
                "lengthChange": true,
                "paging": true,
                "processing": true, // for show progress bar
                "filter": true, // this is for disable filter (search box)
                "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],

                "data": result.data,
                "columns": [
                    { data: "id", title: "Id" },
                    { data: "fullName", title: "Full Name" },
                    {
                        data: null, title: "Mobile No.",
                        render: function (data, type, row) {
                            return row.mobile1 + ' <br> ' + row.mobile2 + ' <br> ' + row.mobile3;
                        }
                    },
                    {
                        data: null, title: "Email", render: function (data, type, row) {
                            return row.email1 + ' <br> ' + row.email2;
                        }
                    },
                    { data: "totalExperienceAnnual", title: "Total Exp. (In Year)" },
                    { data: "relevantExperienceYear", title: "Relevant Exp. (In Year)" },
                    { data: "currentDesignation", title: "Designation" },
                    { data: "skills", title: "Skills" },
                    {
                        data: null,
                        title: 'Action',
                        render: function (data, type, row) {
                            if (row.flowStatus == "Interview_Info") {
                                return '<button type="button" class="btn btn-info btn-block" data-toggle="modal" data-target="#modal-default" onclick="ShowScheduleInterview(' + row.id + ',\'' + row.fullName + '\',true)"><i class="fa fa-user-tie"></i> Interview Info</button>';
                            } else if (row.flowStatus == "Offer") {
                                return '<div class="btn-group"><button type="button" class="btn btn-primary" data-toggle="modal" data-target="#modal-default" onclick="ShowScheduleInterview(' + row.id + ',\'' + row.fullName + '\',false)"><i class="fa fa-box-open"></i> Offer Given</button><button type="button" class="btn btn-primary dropdown-toggle dropdown-icon" data-toggle="dropdown" aria-expanded="false"><span class="sr-only">Toggle Dropdown</span></button><div class="dropdown-menu" role="menu" style=""><a id="1" class="dropdown-item text-info" onclick="ShowUpdateHireStatusAlert(this, ' + row.id + ')">To Be Join</a><a id="2" class="dropdown-item text-success" onclick="ShowUpdateHireStatusAlert(this, ' + row.id + ')">Join</a><a id="3" class="dropdown-item text-danger" onclick="ShowUpdateHireStatusAlert(this, ' + row.id + ')">No Show</a></div></div>';
                            } else if (row.flowStatus == "To_Be_Join") {
                                return '<div class="btn-group"><button type="button" class="btn btn-secondary" data-toggle="modal" data-target="#modal-default" onclick="ShowScheduleInterview(' + row.id + ',\'' + row.fullName + '\',false)"><i class="fa fa-clock"></i> To Be Join</button><button type="button" class="btn btn-secondary dropdown-toggle dropdown-icon" data-toggle="dropdown" aria-expanded="false"><span class="sr-only">Toggle Dropdown</span></button><div class="dropdown-menu" role="menu" style=""><a id="2" class="dropdown-item text-success" onclick="ShowUpdateHireStatusAlert(this, ' + row.id + ')">Join</a><a id="3" class="dropdown-item text-danger" onclick="ShowUpdateHireStatusAlert(this, ' + row.id + ')">No Show</a></div></div>';
                            } else if (row.flowStatus == "No_Show") {
                                return '<button type="button" class="btn btn-danger btn-block" data-toggle="modal" data-target="#modal-default" onclick="ShowScheduleInterview(' + row.id + ',\'' + row.fullName + '\',false)"><i class="fa fa-user-slash"></i> No Show</button>';
                            } else if (row.flowStatus == "Join") {
                                return '<button type="button" class="btn btn-success btn-block" data-toggle="modal" data-target="#modal-default" onclick="ShowScheduleInterview(' + row.id + ',\'' + row.fullName + '\',false)"><i class="fa fa-briefcase"></i> Joined</button>';
                            }
                        }
                    }]
            }).buttons().container().appendTo('#tbUploadedResumes_wrapper .col-md-6:eq(0)');;
        }
        else {
            $.blockUI({
                message: "<h2>" + result.message + "</p>"
            });
        }
        $.unblockUI();
    });
}

function GetInterviewRoundTypeList() {

    $.blockUI({ message: "<h2>Please wait</p>" });

    $("#ddlInterviewVia").empty();
    $("#ddlInterviewVia").append($("<option selected disabled value='0'>Select Interview Via</option>"));

    ajaxCall("Post", false, '/Common/GetInterviewRoundTypeList', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#ddlInterviewVia").append($("<option></option>").val(value.id).html(value.interviewRoundTypeName));
            })
        } else {
            $.blockUI({
                message: "<h2>" + result.message + "</p>"
            });
        }
        $.unblockUI();
    });
}

function ShowScheduleInterview(candidateId, candidateName, showAddBlock) {
    $("#txtCandidateId").val(candidateId);
    $("#txtCandidateName").val(candidateName);

    if (showAddBlock) {
        $("#dScheduleInterview").show();
    } else {
        $("#dScheduleInterview").hide();
    }

    GetScheduleInterviews(candidateId);
}

function ScheduleInterview() {
    $.blockUI({ message: "<h2>Please wait</p>" });


    $("#btnScheduleInterview").attr("disabled", true);

    var isWalkIn = false;

    if ($('input[name="rdInterviewMedium"]:checked').val() == "WalkIn") {
        isWalkIn = true;
    }

    var requestModel = {
        "ResumeFileUploadId": 1,
        "InterviewRoundId": parseInt($("#txtInterviewRoundId").val()) || 0,
        "ResumeFileUploadDetailId": parseInt($("#txtCandidateId").val()),
        "InterviewRoundTypeId": isWalkIn ? 0 : parseInt($("#ddlInterviewVia").val()),
        "InterviewStartDateTime": $('#txtInterviewDateTime').val(),
        "InterviewBy": $('#txtInterviewerName').val(),
        "InterviewLocation": isWalkIn ? $('#txtLocation').val() : "",
        "Note": $('#txtNote').val(),
    }

    ajaxCall("Post", false, '/UploadedResume/ScheduleInterview', JSON.stringify(requestModel), function (result) {
        if (result.status == true) {
            Toast.fire({ icon: 'success', title: result.message });
            setInterval(function () {
                window.location.reload();
            }, 1500)
        } else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();

        $("#btnScheduleInterview").removeAttr("disabled");
    });
}

function GetScheduleInterviews(CandidateId) {
    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/UploadedResume/GetScheduledInterviewListByResumeId?ResumeId=' + CandidateId, null, function (result) {
        if (result.status == true) {

            if (tbScheduleInterviews !== null) {
                tbScheduleInterviews.destroy();
                tbScheduleInterviews = null;
            }

            tbScheduleInterviews = $('#tbScheduleInterviews').DataTable({
                "responsive": true,
                "lengthChange": true,
                "paging": true,
                "processing": true, // for show progress bar
                "filter": true, // this is for disable filter (search box)
                "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],

                "data": result.data,
                "columns": [
                    { data: "id", title: "Id" },
                    { data: "interviewBy", title: "Interviewer(s)" },
                    {
                        data: null, title: "Interview Date-Time", render: function (data, type, row) {
                            var datetime = new Date(row.interviewStartDateTime);
                            var stDt = datetime.toLocaleString('en-US', { day: '2-digit', month: 'short', year: '2-digit', hour: '2-digit', minute: '2-digit' });
                            datetime = new Date(row.interviewEndDateTime);
                            var edDt = datetime.toLocaleString('en-US', { day: '2-digit', month: 'short', year: '2-digit', hour: '2-digit', minute: '2-digit' });

                            return stDt + " -<br> " + edDt;
                        }
                    },
                    { data: "interviewOn", title: "Interview Medium" },
                    { data: "notes", title: "Notes" },
                    {
                        data: null,
                        title: 'Status',
                        render: function (data, type, row) {
                            if (row.isEditable) {
                                return '<button type="button" class="btn btn-outline-primary" onclick="RescheduleInterview(' + row.id + ',\'' + row.interviewBy + '\',\'' + row.interviewRoundTypeId + '\',\'' + row.interviewOn + '\',\'' + row.interviewStartDateTime + '\',\'' + row.notes + '\')"><i class="fa fa-pen"></i> Re-Schedule</button>';
                            } else {
                                if (row.interviewRoundStatusName == "Scheduled") {
                                    return '<select class="form-control" onchange="UpdateInterviewStatus(' + row.id + ', this)"><option selected disabled value="1">Scheduled</option><option class="text-success" value="2">Cleared</option><option class="text-danger" value="3">Rejected</option><option class="text-warning" value="4">No Show</option></select>';
                                } else if (row.interviewRoundStatusName == "Cleared") {
                                    return '<select class="form-control text-success" onchange="UpdateInterviewStatus(' + row.id + ', this)"><option disabled value="1">Scheduled</option><option class="text-success" disabled selected value="2">Cleared</option><option class="text-danger" value="3">Rejected</option><option class="text-warning" value="4">No Show</option></select>';
                                } else if (row.interviewRoundStatusName == "Rejected") {
                                    return '<select class="form-control text-danger" onchange="UpdateInterviewStatus(' + row.id + ', this)"><option disabled value="1">Scheduled</option><option class="text-success" value="2">Cleared</option><option class="text-danger" disabled selected value="3">Rejected</option><option class="text-warning" value="4">No Show</option></select>';
                                } else if (row.interviewRoundStatusName == "No Show") {
                                    return '<select class="form-control text-warning" onchange="UpdateInterviewStatus(' + row.id + ', this)"><option disabled value="1">Scheduled</option><option class="text-success" value="2">Cleared</option><option class="text-danger" value="3">Rejected</option><option class="text-warning" disabled selected value="4">No Show</option></select>';
                                }
                            }
                        }
                    },
                ]
            }).buttons().container().appendTo('#tbScheduleInterviews_wrapper .col-md-6:eq(0)');;
        }
        else {
            $.blockUI({
                message: "<h2>" + result.message + "</p>"
            });
        }
        $.unblockUI();
    });
}

function RescheduleInterview(interviewRoundId, interviewers, interviewRoundType, interviewLocation, interviewTime, interviewNotes) {

    GetInterviewRoundTypeList();
    $("#btnScheduleInterview").html("Re-Schedule Interview");
    $("#btnScheduleInterview").removeClass("btn-success").addClass("btn-warning mr-2");

    if (interviewRoundType == 0) {
        $('#rdInterviewWalkIn').prop('checked', true);
        $('#rdInterviewRemote').prop('checked', false);

        $("#dWalkIn").show();
        $("#dRemoteLocation").hide();

        $("#txtLocation").val(interviewLocation);
        $("#ddlInterviewVia").val(0);
    } else {
        $('#rdInterviewWalkIn').prop('checked', false);
        $('#rdInterviewRemote').prop('checked', true);

        $("#dWalkIn").hide();
        $("#dRemoteLocation").show();

        $("#txtLocation").val("");
        $("#ddlInterviewVia").val(interviewRoundType);
    }

    $("#txtInterviewRoundId").val(interviewRoundId);
    $("#txtInterviewerName").val(interviewers);
    $("#txtInterviewDateTime").val(interviewTime);
    $("#txtNote").val(interviewNotes);

    $("#btnCancelReschedule").show();
}

function CancelReschedule() {
    $("#btnScheduleInterview").html("Schedule Interview");
    $("#btnScheduleInterview").removeClass("btn-warning mr-2").addClass("btn-success");

    $('#rdInterviewWalkIn').prop('checked', true);

    $("#txtInterviewRoundId").val(0);
    $("#txtInterviewerName").val("");
    $("#txtInterviewDateTime").val("");
    $("#txtLocation").val("");
    $("#txtNote").val("");

    $("#btnCancelReschedule").hide();
}

function UpdateInterviewStatus(interviewRoundId, values) {
    var requestModel = {
        "InterviewRoundId": interviewRoundId,
        "StatusId": values.value,
    }

    ajaxCall("Post", false, '/UploadedResume/UpdateInterviewStatus', JSON.stringify(requestModel), function (result) {
        if (result.status == true) {
            Toast.fire({ icon: 'success', title: result.message });
        } else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}

function ShowUpdateHireStatusAlert(element, uploadedResumeId) {
    swal.fire({
        title: "Update Interview Status",
        html: "Are you sure you want to update the interview status to <br><b>'" + $(element).text() + "'</b>?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'Cancel',
    })
        .then((result) => {
            console.log(result)
            if (result.isDismissed === false) {
                if (result.isConfirmed) {
                    UpdateHireStatus(element.id, uploadedResumeId);
                }
            } else {
                Toast.fire({ icon: 'warning', title: "Interview status update abort!" });
            }
        });
}

function UpdateHireStatus(hireStatusId, uploadedResumeId) {
    var requestModel = {
        "HireStatusId": hireStatusId,
        "UploadedResumeId": uploadedResumeId,
    }

    ajaxCall("Post", false, '/UploadedResume/UpdateInterviewHireStatus', JSON.stringify(requestModel), function (result) {
        if (result.status == true) {
            Toast.fire({ icon: 'success', title: result.message });
            setInterval(function () {
                window.location.reload();
            }, 1500)
        } else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}