$(document).ready(function () {
    alert("Page loading");

    $('#txtActionDate').datepicker({
        dateFormat: 'dd-mm-yy',
    });
    $('#txtNextFollowupDate').datepicker({
        dateFormat: 'dd-mm-yy',
    });
    LoadActions();
    LoadStatus();

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

$("#btnSubmitAction").click(function () {
    if (validateRequiredFieldsByGroup("modalActionUpload")) {

        ajaxCallWithoutDataType("GET", false, '/SalesLead/ActionTaken', null, function (result) {
            if (result.status == true) {
                

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

