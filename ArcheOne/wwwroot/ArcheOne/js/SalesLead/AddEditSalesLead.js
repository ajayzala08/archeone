var isPageLoad = 0;
$(document).ready(function () {
    /* EditMode = ($('#txtSalesLeadId').val() != undefined && $('#txtSalesLeadId').val() > 0) ? 1 : 0;*/
    LoadCountry();

    $("#ddlCountry").change(function () {
        LoadStateByCountryId($(this).val());

    });

    $("#ddlState").change(function () {
        LoadCityByStateId($(this).val());
    });

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






function LoadCountry() {
    ajaxCallWithoutDataType("GET", false, '/SalesLead/Countries', null, function (result) {
        if (result.status == true) {
            $("#ddlCountry").html('');
            $("#ddlCountry").append($("<option></option>").val(0).html('---Select---'));
            $.each(result.data, function (data, value) {
                $("#ddlCountry").append($("<option></option>").val(value.id).html(value.countryName));
            })
        }
        var selectedCountryId = ($("#txtSelectedCountyId").val() != undefined && $("#txtSelectedCountyId").val() > 0 && isPageLoad <= 0) ? $("#txtSelectedCountyId").val() : 0;
        $("#ddlCountry").val(selectedCountryId);
        $("#ddlCountry").change();
    });

}
function LoadStateByCountryId(countryId) {


    ajaxCallWithoutDataType("GET", false, '/SalesLead/States?id=' + countryId, null, function (result) {
        if (result.status == true) {

            $("#ddlState").html('');
            $("#ddlState").append('<option value="0">--- Select ---</option>');
            $.each(result.data, function (data, value) {
                $("#ddlState").append($("<option></option>").val(value.id).html(value.stateName));
            })
        }
        var selectedStateId = ($("#txtSelectedStateId").val() != undefined && $("#txtSelectedStateId").val() > 0 && isPageLoad <= 0) ? $("#txtSelectedStateId").val() : 0;
        
        $("#ddlState").val(selectedStateId);
        $("#ddlState").change();



    });
}
function LoadCityByStateId(StateId) {

    $("#ddlCity").html('');
    $("#ddlCity").append($("<option></option>").val(0).html('---Select---'));
    ajaxCallWithoutDataType("GET", false, '/SalesLead/Cities?id=' + StateId, null, function (result) {
        if (result.status == true) {

            $.each(result.data, function (data, value) {
                $("#ddlCity").append($("<option></option>").val(value.id).html(value.cityName));
            })
        }
        var selectedCityId = ($("#txtSelectedCityId").val() != undefined && $("#txtSelectedCityId").val() > 0 && isPageLoad <= 0) ? $("#txtSelectedCityId").val() : 0;
        $("#ddlCity").val(selectedCityId);
        isPageLoad = 1;
    });
}
