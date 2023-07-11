var EditMode = 1;
$(document).ready(function () {


    $("#txtEndDate").change();
    $('.select2').select2()
   
    $("#btnSaveAdd").click(function () {
        SaveUpdateLeave();
    });

    $("#btnCancel").click(function () {
        window.location.href = '/Leaves/Leaves';
    });

    $("#txtEndDate").change(function () {
        let EndDateData = {
            "StartDate": $('#txtStartDate').val(),
            "EndDate": $("#txtEndDate").val(),
            "StartTime": $('#ddlStartTime').find('option:selected').text(),
        }
        //LoadEndTime($(this).val());
        $("#ddlEndTime").empty();

        /*if (txtSelectedEndTime != "0") {*/
        //if (!isNaN(txtSelectedEndTime)) {
            var ddlEndTime = $('#ddlEndTime');
            ddlEndTime.empty();
            ddlEndTime.append($("<option></option>").val('').html('Please wait ...'));
            ajaxCall("Post", false, '/Leaves/EndTimeList', JSON.stringify(EndDateData), function (response) {


                $("#ddlEndTime").html('');
                $("#ddlEndTime").append('<option value="0">--- Select EndTime ---</option>');
                $.each(response.data, function (i, endtime) {
                    
                    $("#ddlEndTime").append('<option  value="' + endtime.id + '">' +
                        endtime.name + '</option>');
                });
                //$("#ddlEndTime").val(0);
                
               

                //$("#ddlState").change();
            });
        //}
    
        //} else {
        //    //$("#ddlEndTime").html('');
        //    $("#ddlEndTime").append('<option value="0">--- Select EndTime ---</option>');
        //}
    });

    $("#ddlStartTime").change(function () {
        LoadEndTime($(this).val());
        let EndDateData = {
            "StartDate": $('#txtStartDate').val(),
            "EndDate": $("#txtEndDate").val(),
            "StartTime": $('#ddlStartTime').find('option:selected').text(),
        }
        //LoadEndTime($(this).val());
        $("#ddlEndTime").empty();

        /*if (txtSelectedEndTime != "0") {*/
        //if (!isNaN(txtSelectedEndTime)) {
        var ddlEndTime = $('#ddlEndTime');
        ddlEndTime.empty();
        ddlEndTime.append($("<option></option>").val('').html('Please wait ...'));
        ajaxCall("Post", false, '/Leaves/EndTimeList', JSON.stringify(EndDateData), function (response) {


            $("#ddlEndTime").html('');
            $("#ddlEndTime").append('<option value="0">--- Select EndTime ---</option>');
            $.each(response.data, function (i, states) {
                console.log(response.data);
                $("#ddlEndTime").append('<option  value="' + states.id + '">' +
                    states.name + '</option>');
            });
            //$("#ddlEndTime").val(0);
            //if (txtSelectedEndTime != null && txtSelectedEndTime != 0 && EditMode == 1) {
            //    $("#ddlEndTime").val(txtSelectedEndTime);
            //   // EditMode = 0;
            //}

            //$("#ddlState").change();
        });
        //}

        //} else {
        //    //$("#ddlEndTime").html('');
        //    $("#ddlEndTime").append('<option value="0">--- Select EndTime ---</option>');
        //}
    });

});
function SaveUpdateLeave() {
    let saveLeavesData = {
        "Id": parseInt($("#txtLeaveId").val()),
        "LeaveTypeId": parseInt($("#ddlLeaveTypeId").val()),
        "LeaveStatusId": isNaN(parseInt($("#ddlLeaveStatusId").val())) ? 0 : parseInt($("#ddlLeaveStatusId").val()),
        "StartDate": $('#txtStartDate').val(),
        "EndDate": $("#txtEndDate").val(),
        "StartTime": $('#ddlStartTime').find('option:selected').text(),
        "EndTime": $('#ddlEndTime').find('option:selected').text(),
        "Reason": $("#txtReason").val(),


    }
    
    console.log(saveLeavesData);
    if (validateRequiredFields()) {
        ajaxCall("Post", false, '/Leaves/SaveUpdateLeave', JSON.stringify(saveLeavesData), function (result) {

            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                RedirectToPage("/Leaves/Leaves");
                GetFilteredLeaveList()
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
                $.unblockUI();
            }
        });
    }
}


function LoadEndTime(EndDateData) {
    $("#ddlEndTime").empty();
    console.log(txtSelectedEndTime)
    if (txtSelectedEndTime != "0") {
        if (!isNaN(txtSelectedEndTime)) {
            var ddlEndTime = $('#ddlEndTime');
            ddlEndTime.empty();
            ddlEndTime.append($("<option></option>").val('').html('Please wait ...'));

            ajaxCall("Get", false, '/Leaves/EndTimeList?id=' + txtSelectedEndTime, null, function (response) {

                $("#ddlEndTime").html('');
                $("#ddlEndTime").append('<option value="0">--- Select EndTime ---</option>');
                $.each(response.data, function (i, states) {
                    console.log(response.data);
                    $("#ddlEndTime").append('<option  value="' + states.id + '">' +
                        states.name + '</option>');
                });
                //$("#ddlEndTime").val(0);
                //if (txtSelectedEndTime != null && txtSelectedEndTime != 0 && EditMode == 1) {
                //    $("#ddlEndTime").val(txtSelectedEndTime);
                //   // EditMode = 0;
                //}

                //$("#ddlState").change();
            });
        }

    } else {
        //$("#ddlEndTime").html('');
        $("#ddlEndTime").append('<option value="0">--- Select EndTime ---</option>');
    }

}
