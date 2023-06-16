$(document).ready(function () {

    $("#btnSaveAdd").click(function () {
        SaveUpdateHoliday();
    });

    $("#btnCancel").click(function () {
        window.location.href = '/Holiday/Holiday';
    });
});

function SaveUpdateHoliday() {
    debugger
    var saveHolidayData = {
        "Id": parseInt($("#txtHolidayId").val()),
        "HolidayName": $("#txtHolidayName").val(),
        "HolidayDate": $("#txtHolidayDate").val(),
    }
    console.log(saveHolidayData);
    debugger
    if (validateRequiredFields()) {
        ajaxCall("Post", false, '/Holiday/SaveUpdateHoliday', JSON.stringify(saveHolidayData), function (result) {
            debugger
            if (result.status == true) {
                Popup_Toast.fire({ icon: 'success', title: result.message });
                $("#btnCancel").click();
                ClearAll();
                GetFilteredHolidayList();
            }
            else {
                Popup_Toast.fire({ icon: 'error', title: result.message });
                $("#btnCancel").click();
                ClearAll();
                GetFilteredHolidayList();
            }
        });
    }
}