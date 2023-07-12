var EditMode = 1;
$(document).ready(function () {
    GetFilteredLeaveList();

    $("#btnAddLeave").click(function () {
        AddEditLeave(0);
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
        ApplyDatatableResponsive('tblLeave');

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
    console.log(value);
    var data = {
        "Id": parseInt($("#txtCancelLeaveId").val()),
        "Reason" : $("#txtReason").val()
    }
    if (validateRequiredFields()) {
       // console.log(Reason);
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


