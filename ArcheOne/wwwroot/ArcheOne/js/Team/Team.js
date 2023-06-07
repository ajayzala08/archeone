var EditMode = 1;
$(document).ready(function () {
    GetFilteredOrganization();
    $("#btnAddTeam").click(function () {
        AddEditTeam(0);
    });
});

$('#AddTeamPage').click(function () { 
    window.location.href = '../Team/AddEditTeam';
});


function AddEditTeam(Id) {
    debugger
    ajaxCall("Get", false, '/team/AddEditTeam?Id=' + Id, null, function (result) {
        $("#AddData").html(result.responseText);
        $("#btnSubmit").click(function () {
            SaveTeam();
        });

    });
}

function SaveTeam() {
    var saveData = {
        "Id": parseInt($("#txtTeamId").val()),
        "TeamLeadId": $("#ddlTeamLeadID").val(),
        "TeamMemberId": $("#ddlTeamMemberId").val()
    }
    console.log(saveData);
    debugger
    if (validateRequiredFields()) {
        ajaxCall("Post", false, '/Team/SaveUpdateTeam', JSON.stringify(saveData), function (result) {

            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                $("#btnClose").click();
                ClearAll();
                GetFilteredOrganization();
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
            }
        });
    }
}

function GetFilteredOrganization() {

    ajaxCall("Get", false, '/Team/TeamList', null, function (result) {

        $("#divTeamList").html(result.responseText);
        ApplyDatatableResponsive('tblTeam');

        $(".btn-edit").click(function () {

            EditMode = 1;
            Id = $(this).attr('Id');
            AddEditTeam(Id);
        });

        $(".btn-delete").click(function () {
            Id = $(this).attr('Id');
            DeleteTeam(Id);
        });
    });
}

function ClearAll() {
    $("#txtTeamId").val(''),
        $("#ddlTeamLeadID").val(''),
        $("#ddlTeamMemberId").val('')
}




