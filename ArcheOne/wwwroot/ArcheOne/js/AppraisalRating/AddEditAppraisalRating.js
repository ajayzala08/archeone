$(document).ready(function () {

    $("#btnSaveAdd").click(function () {
        debugger
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
        debugger
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
        debugger

        console.log(saveAppraisalRatingData);


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
        }
    }
    else {
        Toast.fire({ icon: 'error', title: "Please Select ." });
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