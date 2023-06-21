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
            var Id = $(this).attr('Id');
            AddEditTeam(Id);
        });

        $(".btn-delete").click(function () {
            Id = $(this).attr('Id');
            DeleteTeam(Id);
        });

    });
}
function AddEditTeam(Id) {
    ajaxCall("Get", false, '/Team/AddEditTeam?id=' + Id, null, function (result) {
        if (Id > 0) {
            RedirectToPage('/Team/AddEditTeam?id=' + Id)
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
    /*   var arraSelect =[]*/
    var saveTeamData = {
        "TeamId": parseInt($("#txtTeamId").val()),
        "TeamLeadId": parseInt($("#ddlTeamLeadId").val()),
        "TeamMemberId": parseInt($("#ddlTeamMemberId").multiselect())

    }
    console.log(saveTeamData);

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
            }
        });
    }
}
function ClearAll() {
    $("#txtTeamId").val(''),
        $("#ddlTeamLeadId").val(),
        $("#ddlTeamMemberId").val()
    
}

function DeleteTeam(Id) {
    if ($("#txtTeamId").value > 0) {
        Id = $("#txtTeamId").value;
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

            ajaxCall("Post", false, '/Team/DeleteTeam?Id=' + Id, null, function (result) {
                if (result.status == true) {
                    Popup_Toast.fire({ icon: 'success', title: result.message });
                    GetFilteredTeamList();
                }
                else {
                    Popup_Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    })
};





