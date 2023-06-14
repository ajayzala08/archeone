var EditMode = 1;
$(document).ready(function () {
    GetUserList(null);
    //GetFilteredUserList();
    $("#btnAddUser").click(function () {
        AddEditUser(0);
    });
    GetRoleList();
});

$('#AddUserPage').click(function () {
    window.location.href = '/User/AddEditUser';
});

function AddEditUser(Id) {
    alert('Hi');
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
    alert("Hii...");
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


function GetRoleList() {
    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/Role/RoleList', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#slRoles").append($("<option></option>").val(value.id).html(value.roleName));
            })
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
        $.unblockUI();
    });
}

var dataTable = null;

function GetUserList(RoleId) {
    //debugger
    ajaxCall("Post", false, '/User/UserListByRoleId?RoleId=' + RoleId, null, function (result) {
        if (result.status == true) {
            //debugger
            if (dataTable !== null) {
                dataTable.destroy();
                dataTable = null;
            }

            dataTable = $('#tblUser').DataTable({
                "responsive": true,
                "lengthChange": false,
                "paging": false,
                "searching": false,
                "processing": true, // for show progress bar
                "filter": true, // this is for disable filter (search box)

                "data": result.data,
                "columns": [
                    //{
                    //    data: null,
                    //    title: 'Action',
                    //    render: function (data, type, row) {
                    //        if (row.isDefaultPermission) {
                    //            return '<input type="checkbox" class="permissionBox" checked value="' + row.id + '">';
                    //        } else {
                    //            return '<input type="checkbox" class="permissionBox" value="' + row.id + '">';
                    //        }
                    //    }
                    //},

                    /*{ data: "id", title: "Id" },*/
                    { data: "companyId", title: "Company" },
                    { data: "roleId", title: "Role" },
                    //{ data: "firstName" + "MiddleName" + "LastName", title: "FullName" },
                    { data: "pincode", title: "Pincode" },
                    { data: "mobile1", title: "Contact No." },
                    { data: "email", title: "Email Address" },
                    { data: "photoUrl", title: "ProfileImage" }

                ]
            });
        }
        //else {

        //    $("#btnUpdatePermission").attr("disabled", true);

        //    $.blockUI({
        //        message: "<h2>" + result.message + "</p>"
        //    });
        //}
        ///*$.unblockUI();*/
    });
}

