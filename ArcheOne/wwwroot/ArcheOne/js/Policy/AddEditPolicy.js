$(document).ready(function () {

    $("#btnSaveAdd").click(function () {
        SaveUpdatePolicy();
    });


    $("#btnCancel").click(function () {
        window.location.href = '/Policy/Policy';
    });
});

function SaveUpdatePolicy() {
    if (window.FormData !== undefined) {
        
        var savePolicyData = new FormData();
        var file = $("#txtPolicyFile").get(0).files[0];
        savePolicyData.append("Id", parseInt($("#txtPolicyId").val()));
        savePolicyData.append("PolicyName", ($("#txtPolicyName").val()));
        savePolicyData.append("PolicyDocumentName", file);
        if (validateRequiredFieldsByGroup('divUploadFile')) {
            $.blockUI();
            ajaxCallWithoutDataType("Post", false, '/Policy/SaveUpdatePolicy', savePolicyData, function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/Policy/Policy");
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    $.unblockUI();
                }
            });
        }

    }
    else
    {
        Toast.fire({ icon: 'error', title: "Please Select ." });
    }
}