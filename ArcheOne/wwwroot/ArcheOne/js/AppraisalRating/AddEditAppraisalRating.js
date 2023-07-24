var confirmAssignee = false;
$(document).ready(function () {

    $("#btnSaveAdd").click(function () {
        SaveUpdateAppraisalRating();
    });
    $("#btnCancel").click(function () {
        window.location.href = '/Appraisal/Appraisal';
    });
    $("#AppraisalList").click(function () {
        window.location.href = '/Appraisal/Appraisal';
    });

    $('#chbIsApprove').click(function () {

        var AppraisalId = $(this).attr('AppraisalId');
        AppraisalRatingApproval(AppraisalId);
    });



});

function SaveUpdateAppraisalRating() {
    if (window.FormData !== undefined) {
        var saveAppraisalRatingData = {
            "Id": parseInt($("#txtAppraisalId").val()),
            "ReportingManagerId": $("#txtReportingManagerId").val(),
            "EmployeeId": $("#txtEmployeeId").val(),
            "QualityOfWork": $("#txtQualityOfWork").val(),
            "GoalNtarget": $("#txtGoalsNachievement").val(),
            "WrittenVerbalSkill": $("#txtWrittenNVerbalSkill").val(),
            "InitiativeMotivation": $("#txtInitiative").val(),
            "TeamWork": $("#txtTeamWorkNLeaderShip").val(),
            "ProblemSolvingAbillity": $("#txtProblemsolveAbility").val(),
            "Attendance": $("#txtAttendanceRegularization").val(),
            "Total": $("#txtTotal").val(),
            "Comment": $("#txtEmployeeComment").val(),

        }


        if (validateRequiredFields()) {

            $.blockUI();
            ajaxCall("Post", false, '/AppraisalRating/SaveUpdateAppraisalRating', JSON.stringify(saveAppraisalRatingData), function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/Appraisal/Appraisal");
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    $.unblockUI();
                }
            });
        } else {
            Toast.fire({ icon: 'error', title: "Please Select ." });
        }


    }

}

function AppraisalRatingApproval(AppraisalId) {

    ajaxCall("Post", false, '/AppraisalRating/AppraisalRatingApproval?AppraisalId=' + AppraisalId, null, function (result) {
        if (result.status == true) {
            Toast.fire({ icon: 'success', title: result.message });
            RedirectToPage("/Appraisal/Appraisal");
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
            $.unblockUI();
        }
    });

}
function textChange(inputTypeId) {

    const val = parseInt($('#' + inputTypeId).val());
    const notParsedVal = $('#' + inputTypeId).val();
    if (val == 0 || val > 10 || notParsedVal.length < $('#' + inputTypeId).attr("minlength") || notParsedVal.length > $('#' + inputTypeId).attr("maxlength")) {

        $("#" + $('#' + inputTypeId).attr("errorspan")).removeClass('d-none');
        $("#" + $('#' + inputTypeId).attr("divcontainer")).addClass('has-error');
        $("#" + $('#' + inputTypeId).attr("id")).addClass('is-invalid');
        return false;
    } else {
        $("#" + $('#' + inputTypeId).attr("errorspan")).addClass('d-none');
        $("#" + $('#' + inputTypeId).attr("divcontainer")).removeClass('has-error');
        $("#" + $('#' + inputTypeId).attr("id")).removeClass('is-invalid');
        return true;
    }
}

function TotalCountChange() {
   
    var sum = parseInt($("#txtQualityOfWork").val()) + parseInt($("#txtGoalsNachievement").val()) + parseInt($("#txtWrittenNVerbalSkill").val()) + parseInt($("#txtInitiative").val()) + parseInt($("#txtTeamWorkNLeaderShip").val()) + parseInt($("#txtProblemsolveAbility").val()) + parseInt($("#txtAttendanceRegularization").val());

    if (sum > 0) {
        $("#txtTotal").val(sum);
    } else {
        $("#txtTotal").val(0);
    }

}