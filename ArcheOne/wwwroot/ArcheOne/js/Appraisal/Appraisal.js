$(document).ready(function () {

    GetAppraisalList();
    $('#AddAppraisal').click(function () {
        AddEditAppraisal(0);
    });
    $("#ddlAppraisalStatus").change(function () {
        GetAppraisalList();
    });
});


function GetAppraisalList() {
    var reqData = {
        "AppraisalStatusId": parseInt($("#ddlAppraisalStatus").val()),
    }

    ajaxCall("Get", false, '/Appraisal/AppraisalList', reqData, function (result) {
        $("#divAppraisalList").html(result.responseText);
        ApplyDatatable('tblAppraisal'); 


        $(".btn-edit").click(function () {

            var Id = $(this).attr('Id');
            AddEditAppraisal(Id);
        });

        $(".btn-delete").click(function () {
            Id = $(this).attr('Id');
            DeleteAppraisal(Id);
        });
        $(".circle").click(function () {
            Id = $(this).attr('Id');
            AppraisalInfo(Id);
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
function AppraisalInfo(Id) {
    window.location.href = '/AppraisalRating/AddEditAppraisalRating?Id=' + Id;

};

