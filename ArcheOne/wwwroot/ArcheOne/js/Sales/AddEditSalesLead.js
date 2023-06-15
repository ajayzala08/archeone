$(document).ready(function () {
    var values = [];
    $("#btnSaveAdd").click(function () {
        SaveSalesLead();
    });

    $("#btnCancel").click(function () {
        window.location.href = '/SalesLead/Sales';
    });

   
});


function SaveSalesLead() {
    var dataList = [];
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
    var contactperson = {
        "FirstName": $('#txtFirstName').val(),
        "LastName": $('#txtLastName').val(),
        "Email": $('#txtEmail').val(),
        "Designation": $('#txtDesignation').val(),
        "Mobile1": $('#txtMobile1').val(),
        "Mobile2": $('#txtMobile2').val(),
        "Linkedinurl": $('#txtLinkedinurl').val()
    }
    dataList.push(contactperson);
   var contactperson1 = {
        "FirstName": $('#txtFirstName1').val(),
        "LastName": $('#txtLastName1').val(),
       "Email": $('#txtSalesLeadContactPersonEmail1').val(),
        "Designation": $('#txtDesignation1').val(),
        "Mobile1": $('#txtMobile11').val(),
        "Mobile2": $('#txtMobile21').val(),
        "Linkedinurl": $('#txtLinkedinurl1').val()
    }

    dataList.push(contactperson1);
    var contactperson2 = {
        "FirstName": $('#txtFirstName2').val(),
        "LastName": $('#txtLastName2').val(),
        "Email": $('#txtSalesLeadContactPersonEmail2').val(),
        "Designation": $('#txtDesignation2').val(),
        "Mobile1": $('#txtMobile12').val(),
        "Mobile2": $('#txtMobile22').val(),
        "Linkedinurl": $('#txtLinkedinurl2').val()
    }

    dataList.push(contactperson2);



    var dataSaveModel = {
        "SaveUpdateSalesLeadContactPersonList": dataList,
        "SaveUpdateSalesLeadDetails": saveData
    }

    console.log(dataSaveModel);

    if (validateRequiredFields() && validateReqField('DivSalesLeadContactPerson2')) {
        ajaxCall("Post", false, '/SalesLead/SaveUpdateSalesLead', JSON.stringify(dataSaveModel), function (result) {

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
    var savelist = [];
    var savedata = {
        "fn": $('#txtFirstName').val(),
        "ln": $('#txtLastName').val(),
        "Email": $('#txtEmail').val(),
        "Des": $('#txtDesignation').val(),
        "Mob1": $('#txtMobile1').val(),
        "Mob2": $('#txtMobile2').val(),
        "Link": $('#txtLinkedinurl').val()
    }
    savelist.push(savedata);
    $('#txtFirstName').val('');
    $('#txtLastName').val('')
    $('#txtEmail').val(''),
        $('#txtDesignation').val(''),
        $('#txtMobile1').val(''),
        $('#txtMobile2').val(''),
        $('#txtLinkedinurl').val('')

    addRow(savelist);
}

function addRow(value) {
    //console.log(value)
    for (var i = 0; i < value.length; i++) {

        $('#tblSalesConatactLead tbody').append('<tr><td>' + value[i].fn + '</td><td>' + value[i].ln + '</td><td>' + value[i].Email + '</td><td>' + value[i].Des + '</td><td>' + value[i].Mob1 + '</td><td>' + value[i].Mob2 + '</td><td>' + value[i].Link + '</td><td> <button class="btn btn-info btn-edit1" Id="@item.Id">Edit</button></td> <td><button class= "btn btn-danger btn-delete1" Id = "@item.Id"> Delete</button ></td ></tr >');
    }
}



