﻿$(document).ready(function () {
    
    LoadRequirementForDDL();
    LoadClientDDL();
    LoadPositionTypeDDL();
    LoadRequirementTypeDDL();
    LoadEmploymentTypeDDL();
    LoadRequirementStatusDDL();

  
    GetFilteredRequirementList();

    $("#btnAddRequirement").click(function () {
        AddEditRequirement(0);
    })

    $(".data-filter").change(function () {
        GetFilteredRequirementList();
    });

});

function GetFilteredRequirementList() {
    var reqData = {
        "RequirementForId": parseInt($("#ddlRequirementFor").val()),
        "ClientId": parseInt($("#ddlClients").val()),
        "PositionTypeId": parseInt($("#ddlPositionType").val()),
        "RequirementTypeId": parseInt($("#ddlRequirementType").val()),
        "EmploymentTypeId": parseInt($("#ddlEmploymentType").val()),
        "RequirementStatusId": parseInt($("#ddlRequirementStatus").val()),
    }
    ajaxCall("Get", false, '/Requirement/RequirementList', reqData, function (result) {
        $("#divRequirementList").html(result.responseText);
        ApplyDatatableResponsive('tblRequirement');
        
        $(".btn-Edit").click(function () {
            var RequirementId = $(this).attr('RequirementId');
            AddEditRequirement(RequirementId);
        });
    });
}

function AddEditRequirement(RequirementId) {
    window.location.href = '/Requirement/AddEditRequirement?RequirementId=' + RequirementId;
}

function LoadRequirementForDDL() {
    ajaxCall("Get", false, '/Common/GetRequirementForList', null, function (result) {
        $("#ddlRequirementFor").html('');
        $("#ddlRequirementFor").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlRequirementFor").append('<option  value="' + value.id + '">' + value.requirementForName + '</option>');
            });
        }
        //$('#ddlRequirementFor').select2();
    });
}

function LoadClientDDL() {
    ajaxCall("Get", false, '/Common/GetClientList', null, function (result) {
        $("#ddlClients").html('');
        $("#ddlClients").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlClients").append('<option  value="' + value.id + '">' + value.clientName + '</option>');
            });
        }
    });
}

function LoadPositionTypeDDL() {
    ajaxCall("Get", false, '/Common/GetPositionTypeList', null, function (result) {
        $("#ddlPositionType").html('');
        $("#ddlPositionType").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlPositionType").append('<option  value="' + value.id + '">' + value.positionTypeName + '</option>');
            });
        }
    });
}

function LoadRequirementTypeDDL() {
    ajaxCall("Get", false, '/Common/GetRequirementTypeList', null, function (result) {
        $("#ddlRequirementType").html('');
        $("#ddlRequirementType").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlRequirementType").append('<option  value="' + value.id + '">' + value.requirementTypeName + '</option>');
            });
        }
    });
}

function LoadEmploymentTypeDDL() {
    ajaxCall("Get", false, '/Common/GetEmploymentTypeList', null, function (result) {
        $("#ddlEmploymentType").html('');
        $("#ddlEmploymentType").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlEmploymentType").append('<option  value="' + value.id + '">' + value.employmentTypeName + '</option>');
            });
        }
    });
}

function LoadRequirementStatusDDL() {
    ajaxCall("Get", false, '/Common/GetRequirementStatusList', null, function (result) {
        $("#ddlRequirementStatus").html('');
        $("#ddlRequirementStatus").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlRequirementStatus").append('<option  value="' + value.id + '">' + value.requirementStatusName + '</option>');
            });
        }
    });
}