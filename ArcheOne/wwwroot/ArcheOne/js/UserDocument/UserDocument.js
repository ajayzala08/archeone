$(document).ready(function () {
    UserDocumentList();
});

var tblUserDocument = null;

function UserDocumentList() {
    $.blockUI({ message: "<h2>Please wait</p>" });
    ajaxCall("Post", false, '/UserDocument/UserDocumentList', null, function (result) {
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
                                var fullName = ' + data.fullName + '
                                return '<i class="fa fa-pen pen btn-edit" style="cursor: pointer;" data-toggle="modal" data-target="#modalUserDocument" onclick="GetUserDocsDetails(' + row.id + ')"></i> | <i class="fa fa-trash trash btn-delete" style="cursor: pointer;" onclick="DeleteUserDocs(' + row.id + ')"></i> | <i class="fas fa-envelope-open offerEnvelop btn-delete" style="cursor: pointer;" onclick="downloadOfferLetterDownload(' + row.id + ')"></i> | <i class="fas fa-envelope-open-text experienceEnvelop btn-delete" style="cursor: pointer;" onclick="downloadExperienceLetterDownload(' + row.id + ')"></i>';
                                                                                                                                                                                                                                                                                                                             
                            } else {
                            }
                        }
                    },
                    //{ data: "id", title: "Id" },
                    { data: "userId", title: "UserId" },
                    { data: "documentTypeId", title: "DocumentType" },
                    //{ data: "document", title: "Document " },
                    {
                        data: null,
                        title: 'Document',
                        render: function (data, type, row) {
                            if (data) {
                                return '<i class="fa fa-download btn-download" value=' + row.document + ' Id=' + row.id + ' onclick="GetUserDocs(' + row.id + ')" aria-hidden="true"></i>';
                            } else {
                                //return '<i class="fa fa-trash trash" value="' + data.id + '" onclick="DeleteUser(@item.Id)"></i>';
                            }
                        }
                    }

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
    $("#btnAddUpdateUserDocument").html("Add");
    $("#btnAddUpdateUserDocument").removeClass("btn-success").addClass("btn-warning");
    if (window.FormData !== undefined) {
        $.blockUI();
        var saveData = new FormData();
        var file = $("#txtDocument").get(0).files[0];
        saveData.append("Id", parseInt($("#txtuserDocumentId").val()));
        saveData.append("UserId", $("#ddlUser").val());
        saveData.append("DocumentTypeId", $("#ddlDocumentType").val());
        saveData.append("Document", file);
        saveData.append("IsActive", false);

        console.log(saveData);
        if (validateRequiredFields()) {
            ajaxCallWithoutDataType("Post", false, '/UserDocument/SaveUpdateUserDocument', saveData, function (result) {
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
        else {
            $.unblockUI();
            Toast.fire({ icon: 'success', title: result.message });
            $("#clearAll").click();
            ClearAll();
        }
    }
    else {
        Toast.fire({ icon: 'error', title: "Please Select Document." });
    }
}

function GetUserDocsDetails(Id) {
    $("#btnAddUpdateUserDocument").html("Update");
    $("#btnAddUpdateUserDocument").removeClass("btn-success").addClass("btn-warning");
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

function loadFile(event) {
    $("#txtDocument").html(event.target.files[0].name);
}

function downloadOfferLetterDownload() {
    alert("Do you want to download Offer letter of this employee??")
    window.open('/UserDocument/DownloadOfferLetter')
}