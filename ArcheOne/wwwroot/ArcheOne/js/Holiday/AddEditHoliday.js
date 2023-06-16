$(document).ready(function () {

    $("#btnSaveAdd").click(function () {
        debugger
        SaveUpdateHoliday();
    });
    debugger
    $("#btnCancel").click(function () {
        window.location.href = '/Holiday/Holiday';
    });

    $(".btn-edit").click(function () {
        EditMode = 1;
        Id = $(this).attr('Id');
        AddEditHoliday(Id);
    });

});

function SaveUpdateHoliday() {
    var saveHolidayData = {
       
        "HolidayName": $("#txtHolidayName").val(),
        "HolidayDate": $("#txtHolidayDate").val()
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