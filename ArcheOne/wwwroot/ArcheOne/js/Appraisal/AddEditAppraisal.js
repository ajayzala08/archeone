﻿$(document).ready(function () {

    $("#btnSaveAdd").click(function () {
        debugger
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
        debugger
        console.log(saveAppraisalData);


        if (validateRequiredFields()) {
            $.blockUI();
            debugger
            ajaxCall("Post", false, '/Appraisal/SaveUpdateAppraisal', JSON.stringify(saveAppraisalData), function (result) {

                if (result.status == true) {
                    Popup_Toast.fire({ icon: 'success', title: result.message });
                    $("#btnCancel").click();
                    ClearAll();
                    GetFilteredAppraisalList();
                }
                else {
                    Popup_Toast.fire({ icon: 'error', title: result.message });
                    $("#btnCancel").click();
                    ClearAll();
                    GetFilteredAppraisalList();
                }
            });
        }
    }
    else
    {
        Toast.fire({ icon: 'error', title: "Please Select ." });
    }
}