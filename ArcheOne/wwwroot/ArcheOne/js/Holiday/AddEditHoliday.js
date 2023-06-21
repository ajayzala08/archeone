$(document).ready(function () {
    $('#txtHolidayDate').datepicker({
        dateFormat: 'dd-mm-yy'
    });

    $("#btnSaveAdd").click(function () {
        SaveUpdateHoliday();
    });


    $("#btnCancel").click(function () {
        window.location.href = '/Holiday/Holiday';
    });
});

function SaveUpdateHoliday() {
 
    var saveHolidayData = {
        "Id": parseInt($("#txtHolidayId").val()),
        "HolidayName": $("#txtHolidayName").val(),
        "HolidayDate": $.datepicker.formatDate("yy-mm-dd", $('#txtHolidayDate').datepicker('getDate')),
    }
    console.log(saveHolidayData);
    

    if (validateRequiredFields()) {
        ajaxCall("Post", false, '/Holiday/SaveUpdateHoliday', JSON.stringify(saveHolidayData), function (result) {

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