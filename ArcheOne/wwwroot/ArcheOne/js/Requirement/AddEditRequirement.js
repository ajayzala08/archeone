var myEditor;
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
            //console.log('Editor was initialized', editor);
            myEditor = editor;
        })
        .catch(err => {
            //console.error(err.stack);
        });

    applyRequiredValidation();

    $("#btnSaveUpdateRequirement").click(function () {
        if (validateRequiredFields()) {
            var jobDescription = myEditor.getData();
            if (jobDescription.trim() != '') {
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
                    "JobDescription": jobDescription,
                    "AssignedUserIds": $("#ddlAssignedUsers").val(),
                    "RequirementStatusId": $("#ddlRequirementStatus").val(),
                    "IsActive": $("#chkIsActive").is(':checked'),
                }
                ajaxCall("Post", false, '/Requirement/SaveUpdateRequirement', JSON.stringify(reqData), function (result) {
                    console.log(result)
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
            else {
                Toast.fire({ icon: 'error', title: "Please enter Job Description!" });
            }

        }

    });

    $("#btnCancelRequirement").click(function () {
        RedirectToPage("/Requirement/Index");
    });

    $("#ddlClients").change(function () {
        ajaxCall("Get", false, '/Requirement/GetJobCode?ClientId=' + $(this).val(), null, function (result) {
            if (result.status == true) {
                $("#txtJobCode").val(result.data);
            }
        });
    });

});
