var EditMode = 1;
$(document).ready(function () {
    GetFilteredLeaveList();

    $("#btnAddLeave").click(function () {
        AddEditLeave(0);
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

        //$(".btn-delete").click(function () {
        //    Id = $(this).attr('Id');
        //    DeleteSalesLead(Id);
        //});

    });
}
