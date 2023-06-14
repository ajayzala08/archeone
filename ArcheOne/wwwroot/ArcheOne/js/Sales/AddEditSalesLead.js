$(document).ready(function () {

    $("#btnSaveAdd").click(function () {
        SaveSalesLead();
    });

    $("#btnCancel").click(function () {
        window.location.href = '/SalesLead/Sales';
    });

    $('#btnAddCP').click(function () {
        addRecord();
    });
});


function SaveSalesLead() {

    var saveData = {
        "Id": parseInt($("#txtSalesLeadId").val()),
        "OrgName": $("#txtOrganizationName").val(),
        "Address": $("#txtSalesLeadAddress").val(),
        "CountryId": $("#ddlCountry").val(),
        "StateId": $("#ddlState").val(),
        "CityId": $("#ddlCity").val(),
        "Phone1": $("#txtPhone1").val(),
        "Phone2": $("#txtPhone2").val(),
        "Email1": $("#txtEmail1").val(),
        "Email2": $("#txtEmail2").val(),
        "WebsiteUrl": $("#txtWebsite").val()
    }
    console.log(saveData);
    debugger
    if (validateRequiredFields()) {
        ajaxCall("Post", false, '/SalesLead/SaveUpdateSalesLead', JSON.stringify(saveData), function (result) {

            if (result.status == true) {
                Popup_Toast.fire({ icon: 'success', title: result.message });
                $("#btnCancel").click();
                ClearAll();
                GetFilteredSalesLeadList();
            }
            else {
                Popup_Toast.fire({ icon: 'error', title: result.message });
            }
        });
    }
}
function addRecord() {
    debugger
    var savedata = {
       "fn": $('#txtFirstName').val(),
        "ln": $('#txtLastName').val(),
        "Email": $('#txtEmail').val(),
        "Des": $('#txtDesignation').val(),
        "Mob1": $('#txtMobile1').val(),
        "Mob": $('#txtMobile2').val(),
        "Link": $('#txtLinkedinurl').val()
    }
    addRow(savedata);
    $('#txtFirstName').val('');
}

function addRow(value) {
    console.log(value)
    $('#tblSalesConatactLead tbody').append('<tr><td>' + value + '</td></tr>');
}
