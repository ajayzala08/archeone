$(document).ready(function () {
    $("#btnSaveUpdateSalesLead").click(function () {
        SaveUpdateSalesLead();
    });

    $("#btnCancelSalesLead").click(function () {
        RedirectToPage("/SalesLead/Index");
    });
});

function SaveUpdateSalesLead() {
    if (validate()) {
        var saveData;
        var saveUpdateSalesLeadDetailModel;
        var saveUpdateSalesLeadContactPersonDetails = [];
        saveUpdateSalesLeadDetailModel = {
            "SalesLeadId": parseInt($("#txtSalesLeadId").val()),
            "OrgName": $("#txtOrganizationName").val(),
            "CountryId": parseInt($("#ddlCountry").val()),
            "StateId": parseInt($("#ddlState").val()),
            "CityId": parseInt($("#ddlCity").val()),
            "Address": $("#txtSalesLeadAddress").val(),
            "Phone1": $("#txtPhone1").val(),
            "Phone2": $("#txtPhone2").val(),
            "Email1": $("#txtEmail1").val(),
            "Email2": $("#txtEmail2").val(),
            "WebsiteUrl": $("#txtWebsite").val(),
            "IsActive": $("#chkIsActive").is(':checked')
        }
        var hasValue1 = hasValue(1);
        var hasValue2 = hasValue(2);
        var hasValue3 = hasValue(3);
        if (hasValue1) {
            var id = 1;
            saveUpdateSalesLeadContactPersonDetails.push({
                "SalesLeadContactPersonId": parseInt($("#txtSalesLeadContactPersonId_" + id).val()),
                "SalesLeadId": parseInt($("#txtSalesLeadId_" + id).val()),
                "FirstName": $("#txtFirstName_" + id).val(),
                "LastName": $("#txtLastName_" + id).val(),
                "Email": $("#txtEmail_" + id).val(),
                "Designation": $("#txtDesignation_" + id).val(),
                "Mobile1": $("#txtMobile1_" + id).val(),
                "Mobile2": $("#txtMobile2_" + id).val(),
                "Linkedinurl": $("#txtLinkedinurl_" + id).val(),
                "IsActive": $("#chkIsActive_" + id).is(':checked')
            });
        }
        if (hasValue2) {
            var id = 2;
            saveUpdateSalesLeadContactPersonDetails.push({
                "SalesLeadContactPersonId": parseInt($("#txtSalesLeadContactPersonId_" + id).val()),
                "SalesLeadId": parseInt($("#txtSalesLeadId_" + id).val()),
                "FirstName": $("#txtFirstName_" + id).val(),
                "LastName": $("#txtLastName_" + id).val(),
                "Email": $("#txtEmail_" + id).val(),
                "Designation": $("#txtDesignation_" + id).val(),
                "Mobile1": $("#txtMobile1_" + id).val(),
                "Mobile2": $("#txtMobile2_" + id).val(),
                "Linkedinurl": $("#txtLinkedinurl_" + id).val(),
                "IsActive": $("#chkIsActive_" + id).is(':checked')
            });
        }
        if (hasValue3) {
            var id = 3;
            saveUpdateSalesLeadContactPersonDetails.push({
                "SalesLeadContactPersonId": parseInt($("#txtSalesLeadContactPersonId_" + id).val()),
                "SalesLeadId": parseInt($("#txtSalesLeadId_" + id).val()),
                "FirstName": $("#txtFirstName_" + id).val(),
                "LastName": $("#txtLastName_" + id).val(),
                "Email": $("#txtEmail_" + id).val(),
                "Designation": $("#txtDesignation_" + id).val(),
                "Mobile1": $("#txtMobile1_" + id).val(),
                "Mobile2": $("#txtMobile2_" + id).val(),
                "Linkedinurl": $("#txtLinkedinurl_" + id).val(),
                "IsActive": $("#chkIsActive_" + id).is(':checked')
            });

        }
        saveData = {
            "saveUpdateSalesLeadDetailModel": saveUpdateSalesLeadDetailModel,
            "saveUpdateSalesLeadContactPersonDetails": saveUpdateSalesLeadContactPersonDetails
        };
        ajaxCall("Post", false, '/SalesLead/SaveUpdateSalesLead', JSON.stringify(saveData), function (result) {
            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                RedirectToPage("/SalesLead/Index");
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
            }
        });
    }
}
function hasValue(Id) {
    var hasValue = false;
    $(".CP-" + Id).each(function (index, value) {
        if ($(this).val() != '') {
            hasValue = true;
        }
    });
    return hasValue;
}
function validate() {
    var isValid = false;
    if (validateRequiredFieldsByGroup('divSalesLead') && validateRequiredFieldsByGroup('divContactPerson1')) {
        var hasValue2 = hasValue(2);
        var hasValue3 = hasValue(3);
        if (hasValue3 && !hasValue2) {
            isValid = false;
            Toast.fire({ icon: 'error', title: "Please Enter Contact Person 2 Details First!" });
        }
        else if (!hasValue2 && !hasValue3) {
            isValid = true;
        }
        else {
            if (hasValue2) {
                if (validateRequiredFieldsByGroup('divContactPerson2')) {
                    isValid = true;
                }
            }
            if (hasValue3) {
                if (validateRequiredFieldsByGroup('divContactPerson3')) {
                    isValid = true;
                }
                else {
                    isValid = false;
                }
            }
        }
    }
    return isValid;
}
