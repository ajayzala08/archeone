$(document).ready(function () {
    GetFilteredTeamList();
    //debugger
    $('#AddTeamPage').click(function () {
        AddEditTeam(0);
    });
    //debugger
    //$("#btnCancel").click(function () {
    //    window.location.href = '/Team/Team';
    //});
});

function GetFilteredTeamList() {
    ajaxCall("Get", false, '/Team/TeamList', null, function (result) {
        $("#divTeamList").html(result.responseText);
        ApplyDatatableResponsive('tblTeam');
        debugger
        $(".btn-edit").click(function () {
            var TeamLeadId = $(this).attr('TeamLeadId');
            AddEditTeam(TeamLeadId);
        });

        $(".btn-delete").click(function () {
            Id = $(this).attr('Id');
            DeleteTeam(Id);
        });

    });
}

function AddEditTeam(id) {
    debugger
    window.location.href = '/Team/AddEditTeam?id=' + id;
}



function SaveUpdateTeam() {
    debugger
    var selected = [];
    $('#ddlTeamMemberId :selected').each(function () {
        selected.push[$(this).val()] = $(this).text();
    });
    /*   var arraSelect =[]*/
    debugger
    var saveTeamData = {
        "TeamId": parseInt($("#txtTeamId").val()),
        "TeamLeadId": parseInt($("#ddlTeamLeadId").val()),
        "TeamMemberId": parseInt($("#ddlTeamMemberId").multiselect())
          

//require(['bootstrap-multiselect'], function (purchase) {
//    $('#mySelect').multiselect();
//});

        //TeamMemberId.each(function () {
        //    arraSelect.push($(this).val());
        //});
            //$("#btnmyCountries").click(function () {
            //    var selected = $("#myCountries option:selected");    /*Current Selected Value*/
            //    var message = "";
            //    var arrSelected = [];      /*Array to store multiple values in stack*/
            //    selected.each(function () {
            //        arrSelected.push($(this).val());    /*Stack the Value*/
            //        message += $(this).text() + " " + $(this).val() + "\n";
            //    });
            //    alert(message);
            //}); 



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
    debugger
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
                debugger
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





