﻿var myEditor;
$(document).ready(function () {

    $('#ddlRequirementFor').select2();
    $('#ddlClients').select2();
    $('#ddlPositionType').select2();
    $('#ddlRequirementType').select2();
    $('#ddlEmploymentType').select2();
    $('#ddlRequirementStatus').select2();
    $('#ddlAssignedUsers').select2();

    ClassicEditor
        .create(document.querySelector('#txtJobDescription'))
        .then(editor => {
            console.log('Editor was initialized', editor);
            myEditor = editor;
        })
        .catch(err => {
            console.error(err.stack);
        });

    $("#btnSaveUpdateRequirement").click(function () {
        if (validateRequiredFields()) {
            $.blockUI();
            var reqData = {
                "RequirementId": $("#txtRequirementId").val(),
                "RequirementForId": $("#ddlRequirementFor").val(),
                "ClientId": $("#ddlClients").val(),
                "JobCode": $("#txtJobCode").val(),
                "MainSkill": $("#txtMainSkills").val(),
                "NoOfPosition": $("#txtNoOfPosition").val(),
                "Location": $("#txtLocation").val(),
                "EndClient": $("#txtEndClient").val(),
                "TotalMinExperience": $("#txtTotalMinExperience").val(),
                "TotalMaxExperience": $("#txtTotalMaxExperience").val(),
                "RelevantMinExperience": $("#txtRelevantMinExperience").val(),
                "RelevantMaxExperience": $("#txtRelevantMaxExperience").val(),
                "ClientBillRate": $("#txtClientBillRate").val(),
                "CandidatePayRate": $("#txtCandidatePayRate").val(),
                "PositionTypeId": $("#ddlPositionType").val(),
                "RequirementTypeId": $("#ddlRequirementType").val(),
                "EmploymentTypeId": $("#ddlEmploymentType").val(),
                "Pocname": $("#txtPOCName").val(),
                "MandatorySkills": $("#txtMandatorySkills").val(),
                "JobDescription": myEditor.getData(),
                "AssignedUserIds": $("#ddlAssignedUsers").val(),
                "RequirementStatusId": $("#ddlRequirementStatus").val(),
                "IsActive": $("#chkIsActive").is(':checked'),
            }
            console.log(reqData);
            debugger
            ajaxCall("Post", false, '/Requirement/SaveUpdateRequirement', JSON.stringify(reqData), function (result) {
                console.log(result)
                debugger
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/Requirement/Index");
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    $.unblockUI();
                }
            });
        }

    });

    $("#btnCancelRequirement").click(function () {
        RedirectToPage("/Requirement/Index");
    });
});

