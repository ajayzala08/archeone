$(document).ready(function () {
    $('.select2').select2()
    loader_on();
    $("#btnSubmitAction").show();
    $("#btnUpdateAction").hide();
    $(".modal-title").text("Add Action");
    FolloupActions();
    LoadActions();
    LoadStatus();
    LoadLeadNContactPersonDetails();
    $.unblockUI();
});
function LoadActions() {
    ajaxCallWithoutDataType("GET", false, '/SalesLead/ActionTaken', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#ddlActionPerformed").append($("<option></option>").val(value.id).html(value.action));
                $("#ddlActionPerformedNextFollowupActionPerformed").append($("<option></option>").val(value.id).html(value.action));
            })

        }
    });
}
function LoadStatus() {
    ajaxCallWithoutDataType("GET", false, '/SalesLead/OrganizationStatus', null, function (result) {
        if (result.status == true) {
            $.each(result.data, function (data, value) {
                $("#ddlOrganizationStatus").append($("<option></option>").val(value.id).html(value.status));
            })

        }
    });
}
function LoadLeadNContactPersonDetails() {
    
    const urlParams = new URLSearchParams(window.location.search);
    const id = parseInt(urlParams.get('id'));
    ajaxCallWithoutDataType("GET", false, '/SalesLead/LeadNContactPersonDetails/' + id, null, function (result) {
        if (result.status == true) {
            $("#txtSalesLeadId").val(result.data.leadId);
            $("#txtSalesContactPersonId").val(result.data.contactPersonId);
            $("#lblContactPerson").text(result.data.contactPersonName);
            $("#lblDesignation").text(result.data.designation);
            $("#lblOrganization").text(result.data.leadName);
            $("#lblWebsite").text(result.data.website);
            $("#lblSpeciality").text(result.data.speciality);
            $("#lblCountry").text(result.data.country);
            $("#lblOfficePhone").text(result.data.officePhone);
            $("#lblMobile").text(result.data.mobile);
            $("#lblEmail").text(result.data.email);

        }
    });
}
function FolloupActions() {
    if (validateRequiredFieldsByGroup("modal")) {
        loader_on();
        const urlParams = new URLSearchParams(window.location.search);
        const id = parseInt(urlParams.get('id'));
        ajaxCall("GET", false, '/SalesLead/SalesLeadFollowUpList?id='+id, null, function (result) {
            if (result.status == true) {

                $('#tblSalesLeadFolloup').DataTable({
                    "destroy": true,
                    "responsive": true,
                    "lengthChange": true,
                    "paging": true,
                    "searching": true,
                    "processing": true, // for show progress bar
                    "dom": 'Blfrtip',
                    // "retrieve": true,
                    "filter": true, // this is for disable filter (search box)
                    "data": result.data,
                    "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],
                    "columns": [
                        {
                            class: 'clsWrap',
                            data: null,
                            title: 'Action',
                            render: function (data, type, row) {
                                return '<i class="fa fa-pen pen" style="cursor: pointer;" data-toggle="modal" data-target="#modalActionUpload" value="' + data.id + '" onclick="EditFollowUpAction(' + data.id + ')"></i> | <i class="fa fa-trash trash" style="cursor: pointer;" value="' + data.id + '" onclick="DeleteFollowUpAction(' + data.id + ')"></i>';
                            }
                        },
                        { data: "organization", title: "Organization" },
                        { data: "action", title: "Follow Up" },
                        {
                            data: "followUpDate",
                            type: "date",
                            title: "FollowUp Date",
                            render: function (followUpDate) {
                                let date = new Date(followUpDate);
                                let month = date.getMonth() + 1;
                                return  date.getDate()+ "-" + (month.length > 1 ? month : "0" + month) + "-" + date.getFullYear() ;//dd/MM/yyyy format
                                //date formate with time dd/MM/yyyy hh:mm return (month.length > 1 ? month : "0" + month) + "/" + date.getDate() + "/" + date.getFullYear() + "&nbsp;&nbsp;" + (date.getHours() < 10 ? ("0" + date.getHours()) : date.getHours()) + ":" + (date.getMinutes() < 10 ? ("0" + date.getMinutes()) : date.getMinutes());
                            }
                        },
                        { data: "nextAction", title: "Next Follow Up" },
                        {
                            data: "nextFollowUpDate",
                            type: "date",
                            title: "Next Follow Up Date",
                            render: function (nextFollowUpDate) {
                                let date = new Date(nextFollowUpDate);
                                let month = date.getMonth() + 1;
                                return date.getDate() + "-" + (month.length > 1 ? month : "0" + month) + "-" + date.getFullYear();
                            }
                        },
                        { data: "status", title: "Status" },
                        { data: "contactPerson", title: "Contact Person" }
                    ]
                });
                $.unblockUI();
            }
            else {

                Toast.fire({ icon: 'error', title: result.message });
                $.unblockUI();


            }

        });

    }
}
$("#btnSubmitAction").click(function () {
    if (validateRequiredFieldsByGroup("modalActionUpload")) {

        let salesLeadFollowUpReqModel = {
            "SalesLeadId": parseInt($("#txtSalesLeadId").val()),
            "SalesContactPersonId": parseInt($("#txtSalesContactPersonId").val()),
            "FollowUpDate": $("#txtActionDate").val(),
            "NextFollowUpDate": $("#txtNextFollowupDate").val(),
            "SalesLeadStatusId": parseInt($("#ddlOrganizationStatus option:selected").val()),
            "SalesLeadActionId": parseInt($("#ddlActionPerformed option:selected").val()),
            "SalesLeadNextActionId": parseInt($("#ddlActionPerformedNextFollowupActionPerformed option:selected").val()),
            "Notes": $("#txtActionComment").val(),
            "NextFollowUpNotes": $("#txtNextFollowupActionComment").val()
        }
        ajaxCall("POST", false, '/SalesLead/AddAction', JSON.stringify(salesLeadFollowUpReqModel), function (result) {
           
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    window.location.reload();
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                }
            
        });

    }

});
$('.modal').on('hidden.bs.modal', function () {
    window.location.reload();
});
$("#modalActionUpload").on('hidden', function () {
    $.clearFormFields(this)
});
function DeleteFollowUpAction(ActionId) {
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
            ajaxCall("Post", false, '/SalesLead/DeleteFollowUpAction?id=' + ActionId, null, function (result) {
                if (result.status == true) {
                    FolloupActions();
                    Toast.fire({ icon: 'success', title: result.message });

                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    })

}
function EditFollowUpAction(ActionId) {
    ajaxCall("Post", false, '/SalesLead/EditFollowUpAction?id=' + ActionId, null, function (result) {
        if (result.status == true) {
            let followUpDate = result.data.followUpDateTime.split('T')[0];
            let nextfollowUpDate = result.data.nextFollowUpDateTime.split('T')[0];
            $("#txtFollowupId").val(result.data.id);
            $("#txtActionDate").val(followUpDate);
            $("#txtNextFollowupDate").val(nextfollowUpDate);
            $("#txtActionComment").val(result.data.notes);
            $("#txtNextFollowupActionComment").val(result.data.nextFollowUpNotes);
            $("#ddlOrganizationStatus").val(result.data.salesLeadStatusId);
            $("#ddlActionPerformed").val(result.data.salesLeadActionId);
            $("#ddlActionPerformedNextFollowupActionPerformed").val(result.data.nextFollowUpActionId);
            $("#btnSubmitAction").hide();
            $("#btnUpdateAction").show();
            $(".modal-title").text("Update Action");
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
    });
}
$("#btnUpdateAction").click(function () {

    if (validateRequiredFieldsByGroup("modalActionUpload")) {

        let salesLeadFollowUpEditReqModel = {
            "FollowUpId": parseInt($("#txtFollowupId").val()),
            "FollowUpDate": $("#txtActionDate").val(),
            "NextFollowUpDate": $("#txtNextFollowupDate").val(),
            "SalesLeadStatusId": parseInt($("#ddlOrganizationStatus option:selected").val()),
            "SalesLeadActionId": parseInt($("#ddlActionPerformed option:selected").val()),
            "SalesLeadNextActionId": parseInt($("#ddlActionPerformedNextFollowupActionPerformed option:selected").val()),
            "Notes": $("#txtActionComment").val(),
            "NextFollowUpNotes": $("#txtNextFollowupActionComment").val()
        }
        ajaxCall("POST", false, '/SalesLead/UpdateAction', JSON.stringify(salesLeadFollowUpEditReqModel), function (result) {

            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                window.location.reload();
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
            }

        });

    }

});
$("#btnBackToLead").click(function () {
    window.location.href = '/SalesLead/Index';
});
