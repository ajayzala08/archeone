$(document).ready(function () {
    $.blockUI();

/*    $('#txtActionDate').datepicker({
        dateFormat: 'dd-mm-yy',
    });
    $('#txtNextFollowupDate').datepicker({
        dateFormat: 'dd-mm-yy',
    });*/
    LoadActions();
    LoadStatus();
    LoadLeadNContactPersonDetails();
    FolloupActions();
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
/*    LeadId = x.leadDetail.Id,
        LeadName = x.leadDetail.OrgName,
        Website = x.leadDetail.WebsiteUrl,
        OfficePhone = x.leadDetail.Phone1,
        Speciality = "-",
        Country = "-",
        ContactPersonId = x.contactPerson.Id,
        ContactPersonName = $"{x.contactPerson.FirstName} {x.contactPerson.LastName}",
            Designation = x.contactPerson.Designation,
            Mobile = x.contactPerson.Mobile1,
            Email = x.contactPerson.Email*/
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
            console.log(result.data);
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
        $.blockUI();
        const urlParams = new URLSearchParams(window.location.search);
        const id = parseInt(urlParams.get('id'));
        ajaxCall("GET", false, '/SalesLead/SalesLeadFollowUpList?id='+id, null, function (result) {
            debugger
            if (result.status == true) {
                debugger

                $('#tblSalary').DataTable({
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

                                return '<i class="fa fa-trash trash" value="' + data.id + '" onclick="DeleteSalary(' + row.id + ')"></i> | <i class="fa fa-download btn-download" value="' + data.id + '" onclick="DownloadSalarySlip(' + row.id + ')"></i>';

                            }
                        },

                        { data: "followUpDate", title: "FollowUp Date" },
                        { data: "nextFollowUpDate", title: "Next Follow Up Date" },
                        { data: "action", title: "Action" },
                        { data: "nextAction", title: "NextAction" },
                        { data: "status", title: "Status" },
                        { data: "organization", title: "Organization" },
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
        console.log(salesLeadFollowUpReqModel);

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

