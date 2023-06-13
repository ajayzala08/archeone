$(document).ready(function () {


    GetFilteredRequirementList();

    $("#btnAddRequirement").click(function () {
        
    })
});

function GetFilteredRequirementList() {
    var RequirementForId = 1;
    var ClientId = 0;
    var PositionTypeId = 1;
    var RequirementTypeId = 0;
    var EmploymentTypeId = 0;
    var RequirementStatusId = 0;
    var reqData = {
        "RequirementForId": RequirementForId,
        "ClientId": ClientId,
        "PositionTypeId": PositionTypeId,
        "RequirementTypeId": RequirementTypeId,
        "EmploymentTypeId": EmploymentTypeId,
        "RequirementStatusId": RequirementStatusId,
    }
    console.log(reqData);
    debugger;
    ajaxCall("Get", false, '/Requirement/RequirementList', reqData, function (result) {
        $("#divRequirementList").html(result.responseText);
        ApplyDatatableResponsive('tblRequirement');
    });
}