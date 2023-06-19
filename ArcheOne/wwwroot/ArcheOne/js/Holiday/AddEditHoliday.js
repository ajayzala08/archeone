$(document).ready(function () {

    $("#btnSaveAdd").click(function () {
        SaveUpdateHoliday();
    });
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