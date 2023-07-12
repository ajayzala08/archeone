$(document).ready(function () {
 
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
        "HolidayDate": $('#txtHolidayDate').val(),
    }
    console.log(saveHolidayData);
    

    if (validateRequiredFields()) {
        ajaxCall("Post", false, '/Holiday/SaveUpdateHoliday', JSON.stringify(saveHolidayData), function (result) {

            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                RedirectToPage("/Holiday/Holiday");
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
                $.unblockUI();
            }
        });
    }
}