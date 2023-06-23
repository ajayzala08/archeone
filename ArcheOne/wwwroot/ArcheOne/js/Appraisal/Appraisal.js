﻿$(document).ready(function () {
    GetAppraisalList();
    $('#AddAppraisal').click(function () {
      
        AddEditAppraisal(0);
    });

});

function GetAppraisalList() {

    ajaxCall("Get", false, '/Appraisal/AppraisalList', null, function (result) {
        $("#divAppraisalList").html(result.responseText);
        ApplyDatatableResponsive('tblAppraisal');

        $(".btn-edit").click(function () {

            var Id = $(this).attr('Id');
            AddEditAppraisal(Id);
        });

        $(".btn-delete").click(function () {
            Id = $(this).attr('Id');
            DeleteAppraisal(Id);
        });

    });
}


function AddEditAppraisal(Id) {
   
    window.location.href = '/Appraisal/AddEditAppraisal?Id=' + Id;
}

function DeleteAppraisal(Id) {
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

            ajaxCall("Post", false, '/Appraisal/DeleteAppraisal?Id=' + Id, null, function (result) {

                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    GetAppraisalList();
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                }

            });
        }
    })
};


