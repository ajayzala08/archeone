$(document).ready(function () {

    
    UserDocumentList();

    $("#btnAddUpdateUserDocument").click(function () {
        AddUpdateUserDocument();
    });
    $("#btnCancel").click(function () {
        window.location.href = '/UserDocument/UserDocument';
    });
});

var tblUserDocument = null;


function UserDocumentList() {
    $.blockUI({ message: "<h2>Please wait</p>" });

    ajaxCall("post", false, '/UserDocument/UserDocumentList', null, function (result) {
        if (result.status == true) {

            if (tblUserDocument !== null) {
                tblUserDocument.destroy();
                tblUserDocument = null;
            }

            tblUserDocument = $('#tblUserDocument').DataTable({
                "responsive": true,
                "lengthChange": true,
                "paging": true,
                "processing": true,
                "filter": true,
                "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],

                "data": result.data,
                "columns": [
                    {
                        class: 'clsWrap',
                        data: null,
                        title: 'Action',

                        render: function (data, type, row) {
                            if (data) {
                                
                                return '<i class="fa fa-pen pen btn-edit" style="cursor: pointer;" data-toggle="modal" data-target="#modalUserDocument" onclick="GetUserDocsDetails(' + row.id + ')"></i> | <i class="fa fa-trash trash btn-delete" style="cursor: pointer;" onclick="DeleteUserDocs(' + row.id + ')"></i> | <i class="fa fa-download btn-download" style="cursor: pointer;" value=' + row.document + ' Id=' + row.id + ' onclick="GetUserDocs(' + row.id + ')" aria-hidden="true"></i>';

                            }

                        }
                    },

                    { data: "employeeCode", title: "EmployeeCode" },
                    { data: "employeeName", title: "EmployeeName" },
                    { data: "documentTypeId", title: "DocumentType" },


                ]
            }).buttons().container().appendTo('#tblUserDocument_wrapper .col-md-6:eq(0)');
        }
        else {
            $.blockUI({
                message: "<h2>" + result.message + "</p>"
            });
        }
        $.unblockUI();
    });


}

function GetUserDocs(id) {
    
    window.open('/UserDocument/GetUserDocument?Id=' + id, "_blank");
};

function AddUpdateUserDocument() {
    
    if (window.FormData !== undefined) {
        var saveUserDocumentData = new FormData();
        var file = $("#txtDocument").get(0).files[0];
        saveUserDocumentData.append("Id", parseInt($("#txtuserDocumentId").val()));
        saveUserDocumentData.append("UserId", parseInt($("#ddlUser").val()));
        saveUserDocumentData.append("DocumentTypeId", parseInt($("#ddlDocumentType").val()));
        saveUserDocumentData.append("Document", file);

        console.log(saveUserDocumentData);

        if (validateRequiredFieldsByGroup('divUploadFile')) {
            $.blockUI();
            ajaxCallWithoutDataType("Post", false, '/UserDocument/SaveUpdateUserDocument', saveUserDocumentData, function (result) {
                console.log(result);
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/UserDocument/UserDocument");
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    $.unblockUI();
                }
            });
        }
        
    }
    else {
        Toast.fire({ icon: 'error', title: "Please Select ." });
    }
}

function GetUserDocsDetails(Id) {
    
    $(".modal-title").text("Edit UserDocument");
    $("#btnAddUpdateUserDocument").removeClass("btn-success").addClass("btn-warning");
    $("#btnAddUpdateUserDocument").html("Update");
    
    ajaxCall("Post", false, '/UserDocument/GetUserDocumentById?Id=' + Id, null, function (result) {
        if (result.status == true) {
            $("#txtuserDocumentId").val(result.data.id);
            $("#ddlUser").val(result.data.userId);
            $('#ddlDocumentType').val(result.data.documentTypeId);
            $("#txtDocument").val(result.data.document);
        }
        else {
            message: "<h2>" + result.message + "</p>"
        }
    });
}

function CancelDocument() {
    $("#txtuserDocumentId").val(0);
    $('#ddlUser').val(0);
    $("#ddlDocumentType").val(0);
    $("#txtDocument").val(0);

    $("#btnAddUpdateUserDocument").html("Save");
    $("#btnAddUpdateUserDocument").removeClass("btn-warning").addClass("btn-success");

}

function DeleteUserDocs(id) {
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
            ajaxCall("Post", false, '/UserDocument/DeleteUserDocument?Id=' + id, null, function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage('/UserDocument/UserDocument');
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    })
};

//function loadFile(event) {
//    $("#txtDocument").html(event.target.files[0].name);
//}

//function downloadConfirmationLetter(id) {
//    window.open('/UserDocument/DownloadConfirmationLetter?Id=' + id);
//}
//function downloadExperienceLetter(id) {
//    window.open('/UserDocument/DownloadExperienceLetter?Id=' + id);
//}

//function sendEmail() {
//    alert("Hii");
//}