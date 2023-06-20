$(document).ready(function () {

    $("#btnSaveAdd").click(function () {
        debugger
        SaveUpdatePolicy();
    });


    $("#btnCancel").click(function () {
        window.location.href = '/Policy/Policy';
    });
});

function SaveUpdatePolicy() {
    debugger
    if (window.FormData !== undefined) {
        $.blockUI();
       /* var file = $("#txtPolicyFile").get(0).files[0];*/
        var savePolicyData = new FormData();
        var file = $("#txtPolicyFile").get(0).files[0];
        savePolicyData.append("Id", parseInt($("#txtPolicyId").val()));
        savePolicyData.append("PolicyName", ($("#txtPolicyName").val()));
        savePolicyData.append("PolicyDocument", file);

        console.log(savePolicyData);
        debugger

        if (validateRequiredFields()) {
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