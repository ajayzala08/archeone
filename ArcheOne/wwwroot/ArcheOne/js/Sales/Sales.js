var EditMode = 1;
$(document).ready(function () {
    GetFilteredSalesLeadList();
    $("#btnAddSaleLead").click(function () {
        AddEditSalesLead(0);
    });
});

$('#btnAddSaleLead').click(function () {
    window.location.href = '/Sales/AddEditSalesLead';
    GetFilteredSalesConatactPersonList();
});

function AddEditSalesLead(Id) {
    ajaxCall("Get", false, '/Sales/AddEditSalesLead?id=' + Id, null, function (result) {
        $("#sectionData").html(result.responseText);
        GetFilteredSalesConatactPersonList();
    });
}

$("#btnSaveAdd").click(function () {
    SaveSalesLead();
});

$("#btnCancel").click(function () {
    window.location.href = '/Sales/Sales';
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
        ajaxCall("Post", false, '/Sales/SaveUpdateSalesLead', JSON.stringify(saveData), function (result) {

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
function ClearAll() {
        $("#txtSalesLeadId").val(''),
        $("#txtOrganizationName").val(),
        $("#txtSalesLeadAddress").val(),
        $("#ddlCountry").val(0),
        $("#ddlState").val(0),
        $("#ddlCity").val(0),
        $("#txtPhone1").val(),
        $("#txtPhone2").val(),
        $("#txtEmail1").val(),
        $("#txtEmail2").val(),
        $("#txtWebsite").val()
}
function GetFilteredSalesLeadList() {

    ajaxCall("Get", false, '/Sales/SalesList', null, function (result) {

        $("#AddSalesLeadData").html(result.responseText);
        ApplyDatatableResponsive('tblSalesLead');

        $(".btn-edit").click(function () {

            EditMode = 1;
            Id = $(this).attr('Id');
            AddEditSalesLead(Id);
        });

        $(".btn-delete").click(function () {
            Id = $(this).attr('Id');
            DeleteSalesLead(Id);
        });

    });
}

function DeleteSalesLead(Id) {
    if ($("#txtSalesLeadId").value > 0) {
        Id = $("#txtSalesLeadId").value;
    }

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

            ajaxCall("Post", false, '/Sales/DeleteSalesLead?Id=' + Id, null, function (result) {

                if (result.status == true) {
                    Popup_Toast.fire({ icon: 'success', title: result.message });
                    GetFilteredSalesLeadList();
                }
                else {
                    Popup_Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    })
};


function GetFilteredSalesConatactPersonList() {

    ajaxCall("Get", false, '/Sales/SalesConatactPersonList', null, function (result) {
        console.log(result.responseText);
        $("#divSalesConatactPersondata").html(result.responseText);
        ApplyDatatableResponsive('tblSalesConatactLead');

        //$(".btn-edit").click(function () {

        //    EditMode = 1;
        //    Id = $(this).attr('Id');
        //    AddEditSalesLead(Id);
        //});

        //$(".btn-delete").click(function () {
        //    Id = $(this).attr('Id');
        //    DeleteSalesLead(Id);
        //});

    });
}

