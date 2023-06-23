$(document).ready(function () {
    var values = [];
    $("#btnSaveUpdateSalesLead").click(function () {
        SaveUpdateSalesLead();
    });

    $("#btnCancelSalesLead").click(function () {
        RedirectToPage("/SalesLead/Sales");
    });


});

function SaveUpdateSalesLead() {
    if (validate()) {
        debugger
    }
    else {
        debugger
    }
}

function validate() {
    debugger
    var isValid = false;
    var hasValue2 = false;
    var hasValue3 = false;
    if (validateRequiredFieldsByGroup('divSalesLead') && validateRequiredFieldsByGroup('divContactPerson1')) {
        $(".CP-2").each(function (index, value) {
            if ($(this).val() != '') {
                hasValue2 = true;
            }
        });
        $(".CP-3").each(function (index, value) {
            if ($(this).val() != '') {
                hasValue3 = true;
            }
        });
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
//function SaveUpdateSalesLead() {
//    var dataList = [];
//    var saveData = {
//        "Id": parseInt($("#txtSalesLeadId").val()),
//        "OrgName": $("#txtOrganizationName").val(),
//        "Address": $("#txtSalesLeadAddress").val(),
//        "CountryId": $("#ddlCountry").val(),
//        "StateId": $("#ddlState").val(),
//        "CityId": $("#ddlCity").val(),
//        "Phone1": $("#txtPhone1").val(),
//        "Phone2": $("#txtPhone2").val(),
//        "Email1": $("#txtEmail1").val(),
//        "Email2": $("#txtEmail2").val(),
//        "WebsiteUrl": $("#txtWebsite").val()
//    }
//    var contactperson = {
//        "SalesLeadContactPersonId": parseInt($("#txtSalesLeadContactPersonId").val()),
//        "SalesLeadId": parseInt($("#txtSalesLeadId").val()),
//        "FirstName": $('#txtFirstName').val(),
//        "LastName": $('#txtLastName').val(),
//        "Email": $('#txtEmail').val(),
//        "Designation": $('#txtDesignation').val(),
//        "Mobile1": $('#txtMobile1').val(),
//        "Mobile2": $('#txtMobile2').val(),
//        "Linkedinurl": $('#txtLinkedinurl').val()
//    }
//    dataList.push(contactperson);
//    var contactperson1 = {
//        "SalesLeadContactPersonId": parseInt($("#txtSalesLeadContactPersonId1").val()),
//        SalesLeadId: parseInt($("#txtSalesLeadId1").val()),
//        "FirstName": $('#txtFirstName1').val(),
//        "LastName": $('#txtLastName1').val(),
//        "Email": $('#txtSalesLeadContactPersonEmail1').val(),
//        "Designation": $('#txtDesignation1').val(),
//        "Mobile1": $('#txtMobile11').val(),
//        "Mobile2": $('#txtMobile21').val(),
//        "Linkedinurl": $('#txtLinkedinurl1').val()
//    }

//    dataList.push(contactperson1);
//    var contactperson2 = {
//        "SalesLeadContactPersonId": parseInt($("#txtSalesLeadContactPersonId2").val()),
//        "SalesLeadId": parseInt($("#txtSalesLeadId2").val()),
//        "FirstName": $('#txtFirstName2').val(),
//        "LastName": $('#txtLastName2').val(),
//        "Email": $('#txtSalesLeadContactPersonEmail2').val(),
//        "Designation": $('#txtDesignation2').val(),
//        "Mobile1": $('#txtMobile12').val(),
//        "Mobile2": $('#txtMobile22').val(),
//        "Linkedinurl": $('#txtLinkedinurl2').val()
//    }

//    dataList.push(contactperson2);

//    var dataSaveModel = {
//        "SaveUpdateSalesLeadContactPersonList": dataList,
//        "SaveUpdateSalesLeadDetails": saveData
//    }


//    //if (validateRequiredFieldsByGroup('divSalesLeadContactPerson2')) {

//    if (validateRequiredFieldsByGroup('divSalesLead') && validateRequiredFieldsByGroup('divSalesLeadContactPerson1')) {


//        if ($('#txtFirstName1').val() != '' || $('#txtLastName1').val() != '' || $('#txtSalesLeadContactPersonEmail1').val() != '' | $('#txtDesignation1').val() != '' || $('#txtMobile11').val() != '') {
//            validateRequiredFieldsByGroup('divSalesLeadContactPerson2')

//            //if ($('#txtFirstName1').val() == null && $('#txtFirstName1').val() == '' && $('#txtLastName1').val() == null && $('#txtLastName1').val() == '' && $('#txtSalesLeadContactPersonEmail1').val() == null && $('#txtSalesLeadContactPersonEmail1').val() == '' && $('#txtDesignation1').val() == null && $('#txtDesignation1').val() == '' && $('#txtMobile11').val() == null && $('#txtMobile11').val() == '') {
//            //    validateRequiredFieldsByGroup('divSalesLeadContactPerson2')
//            //    []
//            alert('success')
//            //ajaxCall("Post", false, '/SalesLead/SaveUpdateSalesLead', JSON.stringify(dataSaveModel), function (result) {

//            //    if (result.status == true) {
//            //        Toast.fire({ icon: 'success', title: result.message });
//            //        $("#btnCancel").click();
//            //        ClearAll();
//            //        GetFilteredSalesLeadList();
//            //    }
//            //    else {
//            //        Toast.fire({ icon: 'error', title: result.message });
//            //    }
//            //});
//        } else {
//            alert('tnamay')
//        }
//    }
//    //}
//    else {
//        alert('error')
//    }

//}

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



