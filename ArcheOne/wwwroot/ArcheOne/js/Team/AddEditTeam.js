$(document).ready(function () {
    $('.select2').select2()

    $("#btnSaveAdd").click(function () {
        debugger
        SaveUpdateTeam();
    });
  
    $("#btnCancel").click(function () {
        window.location.href = '/Team/Team';
    });

    $(".btn-edit").click(function () {
        EditMode = 1;
        Id = $(this).attr('Id');
        AddEditTeam(Id);
    });

});

function SaveUpdateTeam() {
    var saveTeamData = {
        //"TeamId": parseInt($("#txtTeamId").val()),
        "TeamLeadId": parseInt($("#ddlTeamLeadId").val()),
        "TeamMemberId": $("#ddlTeamMemberId").val().map(Number)
    }
    console.log(saveTeamData);
    debugger
    if (validateRequiredFields()) {
        ajaxCall("Post", false, '/Team/SaveUpdateTeam', JSON.stringify(saveTeamData), function (result) {
            debugger
            if (result.status == true) {
                Popup_Toast.fire({ icon: 'success', title: result.message });
                $("#btnCancel").click();
                ClearAll();
                GetFilteredTeamList();
            }
            else {
                Popup_Toast.fire({ icon: 'error', title: result.message });
                $("#btnCancel").click();
                ClearAll();
                GetFilteredTeamList();
            }
        });
    }
}