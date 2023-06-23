$(document).ready(function () {

    $("#btnSaveAdd").click(function () {
        SaveUpdateAppraisal();
    });


    $("#btnCancel").click(function () {
        window.location.href = '/Appraisal/Appraisal';
    });
});

function SaveUpdateAppraisal() {
    if (window.FormData !== undefined) {

        var saveAppraisalData = {
            "Id": parseInt($("#txtAppraisalId").val()),
            "EmployeeId": parseInt($("#ddlEmployeeId").val()),
            "ReportingManagerId": parseInt($("#ddlReportingManagerId").val()),
            "Year": $("#txtYear").val(),
        }

        console.log(saveAppraisalData);


        if (validateRequiredFields()) {
            $.blockUI();
            ajaxCall("Post", false, '/Appraisal/SaveUpdateAppraisal', JSON.stringify(saveAppraisalData), function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/Appraisal/Appraisal");
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    $.unblockUI();
                }
            });
        }
    }
    else {
        Toast.fire({ icon: 'error', title: "Please Select ." });
    }
}