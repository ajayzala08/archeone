var EditMode = 1;
$(document).ready(function () {
    //$('.select2').select2()
    GetFilteredLeaveList();

    $("#btnAddLeave").click(function () {
        AddEditLeave(0);
    });
    $("#btnShowLeave").click(function () {
        $('#modalShowLeave').modal('show');
        ajaxCall("GET", false, '/Leaves/ShowLeavesDetails/', null, function (result) {
            if (result.status == true) {

                $("#lblOpeningLeaveBalance").text(result.data.openingLeaveBalance);
                $("#lblClosingLeaveBalance").text(result.data.closingLeaveBalance);
                $("#lblSickLeaveBalance").text(result.data.sickLeaveBalance);
                $("#lblSickLeaveTaken").text(result.data.sickLeaveTaken);
                $("#lblCasualLeaveTaken").text(result.data.casualLeaveTaken);
                $("#lblCasualLeaveBalance").text(result.data.casualLeaveBalance);
                $("#lblEarnedLeaveTaken").text(result.data.earnedLeaveTaken);
                $("#lblEarnedLeaveBalance").text(result.data.earnedLeaveBalance);
                $("#lblBalanceMonth").text(result.data.balanceMonth);
                $("#lblBalanceYear").text(result.data.balanceYear);

            }
        });
    });


  
    $("#btnUpdateCancelLeave").click(function () {
       
        UpdateCancelLeave();
    });
   
});


function AddEditLeave(Id) {
    window.location.href = '/Leaves/AddEditLeave?Id=' + Id;
}
function GetFilteredLeaveList() {

    ajaxCall("Get", false, '/Leaves/LeavesList', null, function (result) {

        $("#divLeaveList").html(result.responseText);
        ApplyDatatable('tblLeave');

        $(".btn-edit").click(function () {

            EditMode = 1;
            Id = $(this).attr('Id');
            AddEditLeave(Id);
        });

        $(".fa-times").click(function () {
            $('#modalActionCancle').modal('show');
            var value = $(this).attr('data-value');
            $('#txtCancelLeaveId').val(value);
        });

       

    });
}
function UpdateCancelLeave(value) {
   
    var data = {
        "Id": parseInt($("#txtCancelLeaveId").val()),
        "Reason" : $("#txtReason").val()
    }
    if (validateRequiredFields()) {
     
        ajaxCall("Post", false, '/Leaves/UpdateCancelLeave/', JSON.stringify(data) ,function (result) {

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


