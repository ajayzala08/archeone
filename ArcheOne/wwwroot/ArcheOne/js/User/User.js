var EditMode = 1;
$(document).ready(function () {
    GetFilteredUserList();
    $("#btnAddUser").click(function () {
        AddEditUser(0);
    });
});

$('#AddUserPage').click(function () {
    window.location.href = '/User/AddEditUser';
});

function AddEditUser(Id) {
    ajaxCall("Get", false, '/User/AddEditUser?Id=' + Id, null, function (result) {
        if (Id > 0) {
            RedirectToPage('/User/AddEditUser?Id=' + Id)
            $(".preview img").attr('src');
            $(".preview img").show();
        }
        else {
            RedirectToPage("/User/AddEditUser")
        }
    });
}

function DeleteUser(Id) {
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
            ajaxCall("Post", false, '/User/DeleteUser?Id=' + Id, null, function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage('/User/User');
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    })
};

function GetFilteredUserList() {
    ajaxCall("Get", false, '/User/UserList', null, function (result) {
        $("#divUserList").html(result.responseText);
        ApplyDatatableResponsive('tblUser');
        $(".btn-edit").click(function () {
            EditMode = 1;
            Id = $(this).attr('Id');
            AddEditUser(Id);
        });
        $(".btn-delete").click(function () {
            Id = $(this).attr('Id');
            DeleteUser(Id);
        });
    });
}

