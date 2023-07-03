$(document).ready(function () {
    GetFilteredTeamList();
    $('#AddTeamPage').click(function () {
        AddEditTeam(0);
    });

});

function GetFilteredTeamList() {
    ajaxCall("Get", false, '/Team/TeamList', null, function (result) {
        $("#divTeamList").html(result.responseText);
        ApplyDatatableResponsive('tblTeam');

        $(".btn-edit").click(function () {
            var TeamLeadId = $(this).attr('TeamLeadId');
            AddEditTeam(TeamLeadId);
        });

        $(".btn-delete").click(function () {
            TeamLeadId = $(this).attr('TeamLeadId');
            DeleteTeam(TeamLeadId);
        });

    });
}
function AddEditTeam(TeamLeadId) {
    ajaxCall("Get", false, '/Team/AddEditTeam?id=' + TeamLeadId, null, function (result) {
        if (TeamLeadId > 0) {
            RedirectToPage('/Team/AddEditTeam?id=' + TeamLeadId)
            $(".preview img").attr('src');
            $(".preview img").show();
        }
        else {
            RedirectToPage("/Team/AddEditTeam?id=")
        }
    });
}



function SaveUpdateTeam() {
    var selected = [];
    $('#ddlTeamMemberId :selected').each(function () {
        selected.push[$(this).val()] = $(this).text();
    });

    var saveTeamData = {
        "TeamId": parseInt($("#txtTeamId").val()),
        "TeamLeadId": parseInt($("#ddlTeamLeadId").val()),
        "TeamMemberId": parseInt($("#ddlTeamMemberId").multiselect())

    }
    console.log(saveTeamData);

    if (validateRequiredFields()) {
        ajaxCall("Post", false, '/Team/SaveUpdateTeam', JSON.stringify(saveTeamData), function (result) {
            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                $("#btnCancel").click();
                ClearAll();
                GetFilteredTeamList();
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
            }
        });
    }
}

function DeleteTeam(TeamLeadId) {

    if ($("#ddlTeamLeadId").value > 0) {
        TeamLeadId = $("#ddlTeamLeadId").value;
    }

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            ajaxCall("Post", false, '/Team/DeleteTeam?Id=' + TeamLeadId, null, function (result) {

                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    GetFilteredTeamList();
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                }

            });
        }
    })
};







