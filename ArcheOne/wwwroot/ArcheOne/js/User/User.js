var EditMode = 1;
$(document).ready(function () {
    GetUserList(null);
    //GetFilteredUserList();
    $("#btnAddUser").click(function () {
        AddEditUser(0);
        /*manageUserDetails(0);*/
    });
    $("#btnUploadUserSheet").click(function () {
        if (validateRequiredFieldsByGroup("modelUpload")) {

            if (window.FormData !== undefined) {
                var saveData = new FormData();
                var file = $("#fileUserSheet").get(0).files[0];
                saveData.append("UserSheet", file);
                ajaxCallWithoutDataType("Post", false, '/User/UploadUserSheet', saveData, function (result) {
                    if (result.status == true) {
                        Toast.fire({ icon: 'success', title: result.message });
                        window.location.reload(); //window.location.href = window.location.href;
                    }
                    else {
                        Toast.fire({ icon: 'error', title: result.message });

                    }
                });

            }
            else {
                Toast.fire({ icon: 'error', title: "Please Select Usersheet" });
            }

        }
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

function manageUserDetails(Id) {
    ajaxCall("Post", false, '/UserDetails/AddEditUserDetails?userId=' + Id, null, function (result) {
        if (Id > 0) {
            RedirectToPage('/UserDetails/AddEditUserDetails?userId=' + Id)
        }
        else {
            RedirectToPage("/UserDetails/AddEditUserDetails")
        }
    });
}

function AddEditUserDetails(Id) {
    ajaxCall("Get", false, '/UserDetails/AddEditUserDetails?Id=' + Id, null, function (result) {
        if (Id > 0) {
            RedirectToPage('/UserDetails/AddEditUserDetails?Id=' + Id)
            $(".preview img").attr('src');
            $(".preview img").show();
        }
        else {
            RedirectToPage("/UserDetails/AddEditUserDetails")
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
        ApplyDatatable('tblUser');
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

var dataTable = null;

function GetUserList(RoleId) {
    ajaxCall("Post", false, '/User/UserList', null, function (result) {
        if (result.status == true) {
            if (dataTable !== null) {
                debugger
                dataTable.destroy();
                dataTable = null;
            }
            dataTable = $('#tblUser').DataTable({
                "responsive": true,
                "lengthChange": true,
                "paging": true,
                "searching": true,
                "processing": true, // for show progress bar
                /*"dom": 'Blfrtip',*/
                "filter": true, // this is for disable filter (search box)
                "data": result.data,
                "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],
          
                "columns": [
                    {
                        class: 'clsWrap',
                        data: null,
                        title: 'Action',
                        
                        render: function (data, type, row) {
                            if (data) {
                                var fullName = ' + data.fullName + '
                                return '<i class="fa fa-pen pen" value="' + data.id + '" onclick="AddEditUser(' + row.id + ')"></i> | <i class="fa fa-trash trash" value="' + data.id + '" onclick="DeleteUser(' + row.id + ')"></i> | <i class="fa fa-info-circle circle" value="' + data.id + '" onclick="manageUserDetails(' + row.id + ')"></i>';
                            } else {
                                //return '<i class="fa fa-trash trash" value="' + data.id + '" onclick="DeleteUser(@item.Id)"></i>';
                            }
                        }
                    },
                    { data: "companyId", title: "Company" },
                    { data: "roleId", title: "Role" },
                    { data: "fullName", title: "FullName" },
                    { data: "pincode", title: "Pincode" },
                    { data: "mobile1", title: "Contact No." },
                    { data: "email", title: "Email Address" },
                    {
                        data: null,
                        title: 'ProfileImage',
                        render: function (data, type, row) {
                            if (data) {
                                return '<img src="' + data.photoUrl + '" height="60px" width="60px" alt="Profile Image">';
                            } else {
                                //return '<i class="fa fa-trash trash" value="' + data.id + '" onclick="DeleteUser(@item.Id)"></i>';
                            }
                        }
                    }
                ]
            }).buttons().container().appendTo('#tblUser_wrapper .col-md-6:eq(0)');
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
    });
}

function loadFile(event) {
    $('#lblUserSheet').html(event.target.files[0].name);
}