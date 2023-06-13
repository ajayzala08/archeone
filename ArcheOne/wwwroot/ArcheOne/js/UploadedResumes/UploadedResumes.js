$(document).ready(function () {
    GetUploadedResumes();

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
});

var dataTable = null;

function GetUploadedResumes(RoleId) {

    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/UploadedResume/GetUploadedResumeList?RoleId=' + RoleId, null, function (result) {
        if (result.status == true) {

            if (dataTable !== null) {
                dataTable.destroy();
                dataTable = null;
            }

            dataTable = $('#tbUploadedResumes').DataTable({
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
                            if (row.isDefaultPermission) {
                                return '<button type="button" class="btn btn-primary btn-block"><i class="fa fa-user-tie"></i> Schedule Interview</button>';
                            } else {
                                return '<button type="button" class="btn btn-primary btn-block" data-toggle="modal" data-target="#modal-default" onclick="ShowScheduleInterview(' + row.id + ',\'' + row.fullName + '\')"><i class="fa fa-user-tie"></i> Schedule Interview</button>';
                                //return '<button type="button" class="btn btn-primary btn-block" onclick="ShowScheduleInterview()"><i class="fa fa-user-tie"></i> Schedule Interview</button>';
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

function ShowScheduleInterview(candidateId, candidateName) {
    $("#txtCandidateId").val(candidateId);
    $("#txtCandidateName").val(candidateName);
}

function ScheduleInterview() {
    $.blockUI({ message: "<h2>Please wait</p>" });


    $("#btnScheduleInterview").attr("disabled", true);

    var requestModel = {
        "ResumeFileUploadId": 1,
        "ResumeFileUploadDetailId": parseInt($("#txtCandidateId").val()),
        "InterviewRoundTypeId": parseInt($("#ddlInterviewVia").val()) || 0,
        "InterviewStartDateTime": $('#txtInterviewDateTime').val(),
        "InterviewBy": $('#txtInterviewerName').val(),
        "InterviewLocation": $('#txtLocation').val(),
        "Note": $('#txtNote').val(),
        "CreatedBy": 1,
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

        $("#btnUpdatePermission").removeAttr("disabled");
    });
}