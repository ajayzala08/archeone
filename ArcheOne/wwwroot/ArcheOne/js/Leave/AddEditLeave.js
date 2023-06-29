$(document).ready(function () {
    $('.select2').select2()

    $("#btnSaveAdd").click(function () {
        SaveUpdateLeave();
    });

    $("#btnCancel").click(function () {
        window.location.href = '/Leaves/Leaves';
    });

    $("#ddlStartTime").change(function () {
        alert($(this).val());
        LoadEndTime($(this).val());
    });

});
function SaveUpdateLeave() {
    var saveTeamData = {
        "TeamId": parseInt($("#txtTeamId").val()),
        "TeamLeadId": parseInt($("#ddlTeamLeadId").val()),
        "TeamMemberId": $("#ddlTeamMemberId").val().map(Number)
    }
    debugger
    console.log(saveTeamData);
    if (validateRequiredFields()) {
        debugger
        ajaxCall("Post", false, '/Team/SaveUpdateTeam', JSON.stringify(saveTeamData), function (result) {

            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                RedirectToPage("/Team/Team");
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
                $.unblockUI();
            }
        });
    }
}


function LoadEndTime(txtSelectedEndTime) {
    $("#ddlEndTime").empty();

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

}
