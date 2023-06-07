var EditMode = 1;
$(document).ready(function () {
    GetFilteredOrganization();
    $("#btnAddTeam").click(function () {
        AddEditTeam(0);
    });
});

function AddEditTeam(Id) {
    ajaxCall("Get", false, '/Team/AddEditTeam?Id=' + Id, null, function (result) {
        $('#AddTeam').modal('show');
        $("#AddTeamData").html(result.responseText);
        
        $("#btnSubmit").click(function () {
            SaveTeam();
        });


        //$("#ddlCountry").change(function () {

        //    var CountryId = parseInt($(this).val());
        //    var StateId = parseInt($("#txtSelectedStateId").val());

        //    LoadStateByCountry(CountryId, StateId);
        //});

        //$("#ddlState").change(function () {
        //    var StateId = parseInt($(this).val());
        //    var CityId = parseInt($("#txtSelectedCityId").val());
        //    LoadCityByState(StateId, CityId)
        //});

        //$("#ddlCountry").change();

    });
}

function SaveTeam() {
    var saveData = {
       /* "Id": parseInt($("#txtUserId").val()),*/
        "TeamLeadId": $("#ddlTeamLead").val(),
        "TeamMemberId": $("#ddlTeamMember").val()
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
            DeleteUser(Id);
        });
    });
}

function ClearAll() {
    
        $("#ddlTeamLeadId").val(''),
        $("#ddlTeamMemberId").val(0)
}

