var EditMode = 1;
$(document).ready(function () {
    $("#btnAddLeave").click(function () {
        AddEditLeave(0);
    });
   
});

function AddEditLeave(Id) {
    ajaxCall("Get", false, '/Leaves/AddEditLeave?Id=' + Id, null, function (result) {
        if (Id > 0) {
            RedirectToPage('/Leaves/AddEditLeave?Id=' + Id)
           
        }
        else {
            RedirectToPage("/Leaves/AddEditLeave")
        }
    });
}