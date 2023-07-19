$(document).ready(function () {
    GetFilteredHolidayList();
    $('#AddHoliday').click(function () {
        AddEditHoliday(0);
    });
 
});

function GetFilteredHolidayList() {
    ajaxCall("Get", false, '/Holiday/HolidayList', null, function (result) {
        $("#divHolidayList").html(result.responseText);
        ApplyDatatableResponsive('tblHoliday');
        $('#tblHoliday').on('click', '.btn-edit', function () {
            Id = $(this).attr('Id');
            AddEditHoliday(Id);
        });
        $('#tblHoliday').on('click', '.btn-delete', function () {
            Id = $(this).attr('Id');
            DeleteHoliday(Id);
        });

    });
}

function AddEditHoliday(Id) {
    window.location.href = '/Holiday/AddEditHoliday?Id=' + Id;
}

function DeleteHoliday(Id) {
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

            ajaxCall("Post", false, '/Holiday/DeleteHoliday?Id=' + Id, null, function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    GetFilteredHolidayList();
                }
                else {
                    
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }

    })
};



