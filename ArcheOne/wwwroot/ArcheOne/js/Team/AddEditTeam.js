﻿$(document).ready(function () {
    $('.select2').select2()

    $("#btnSaveAdd").click(function () {
       
        SaveUpdateTeam();
    });
  
    $("#btnCancel").click(function () {
        window.location.href = '/Team/Team';
    });

    $(".btn-edit").click(function () {
        EditMode = 1;
        TeamId = $(this).attr('TeamId');
        AddEditTeam(TeamId);
    });

});

function SaveUpdateTeam() {
    var saveTeamData = {
        "TeamId": parseInt($("#txtTeamId").val()),
        "TeamLeadId": parseInt($("#ddlTeamLeadId").val()),
        "TeamMemberId": $("#ddlTeamMemberId").val().map(Number)
    }
    if (validateRequiredFields()) {
        
        ajaxCall("Post", false, '/Team/SaveUpdateTeam', JSON.stringify(saveTeamData), function (result) {

            if (result.status == true) {
                
                Toast.fire({ icon: 'success', title: result.message });
                RedirectToPage("/Team/Team");
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
                $.unblockUI();
            }
        });
    }
}