﻿$(document).ready(function () {
    GetPolicyList();
    $('#AddPolicy').click(function () {
        AddEditPolicy(0);
    });

});

function GetPolicyList() {
   
    ajaxCall("Get", false, '/Policy/PolicyList', null, function (result) {
        $("#divPolicyList").html(result.responseText);
        ApplyDatatableResponsive('tblPolicy');

        $(".btn-edit").click(function () {

            var Id = $(this).attr('Id');
            AddEditPolicy(Id);
        });

        $(".btn-delete").click(function () {
            Id = $(this).attr('Id');
            DeletePolicy(Id);
        });

        $(".btn-download").click(function () {
            Id = $(this).attr('Id');
            GetPolicyReport(Id);
        });


    });
}
function GetPolicyReport(Id) {
    window.open('/Policy/GetPolicyReport?Id=' + Id, "_blank");  
 
}; 

function AddEditPolicy(Id) {
    window.location.href = '/Policy/AddEditPolicy?Id=' + Id;
}

function DeletePolicy(Id) {
    if ($("#txtId").value > 0) {
        Id = $("#txtId").value;
    }
 
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {

            ajaxCall("Post", false, '/Policy/DeletePolicy?Id=' + Id, null, function (result) {
                
                if (result.status == true) {
                    Popup_Toast.fire({ icon: 'success', title: result.message });
                    GetPolicyList();
                }
                else {
                    Popup_Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    })
};


