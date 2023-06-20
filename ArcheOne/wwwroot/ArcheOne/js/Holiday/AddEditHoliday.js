$(document).ready(function () {
    //$("#txtHolidayDate").datepicker({
    //    format: 'dd/mm/yyyy'
    //});
    $('#txtHolidayDate').datepicker({
        /*  minDate: new Date(),*/
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
    debugger
    var saveHolidayData = {
        "Id": parseInt($("#txtHolidayId").val()),
        "HolidayName": $("#txtHolidayName").val(),
        "HolidayDate": $.datepicker.formatDate("yy-mm-dd", $('#txtHolidayDate').datepicker('getDate'))//$("#txtHolidayDate").datepicker({ dateFormat: 'yyyy-mm-dd' }).val()
       /* "HolidayDate": getDateFromFields("txtHolidayDate"),*/
    }
    console.log(saveHolidayData);
    debugger

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