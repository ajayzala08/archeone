﻿var EditMode = 1;
$(document).ready(function () {
    GetFilteredSalesLeadList();

    $("#btnAddSaleLead").click(function () {
        AddEditSalesLead(0);
        
    });


});


function AddEditSalesLead(salesLeadId) {
    RedirectToPage('/SalesLead/AddEditSalesLead?SalesLeadId=' + salesLeadId);
 
}

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
    if (validateRequiredFields()) {
        ajaxCall("Post", false, '/SalesLead/SaveUpdateSalesLead', JSON.stringify(saveData), function (result) {

            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                $("#btnCancel").click();
                ClearAll();
                GetFilteredSalesLeadList();
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
            }
        });
    }
}

function ClearAll() {
    $("#txtSalesLeadId").val('');
    $("#txtOrganizationName").val('');
    $("#txtSalesLeadAddress").val();
    $("#ddlCountry").val(0);
    $("#ddlState").val(0);
    $("#ddlCity").val(0);
    $("#txtPhone1").val();
    $("#txtPhone2").val();
    $("#txtEmail1").val();
    $("#txtEmail2").val();
    $("#txtWebsite").val();
    $('#txtFirstName').val();
    $('#txtLastName').val();
    $('#txtEmail').val();
    $('#txtDesignation').val();
    $('#txtMobile1').val();
    $('#txtMobile2').val();
    $('#txtLinkedinurl').val();
    $('#txtFirstName1').val();
    $('#txtLastName1').val();
    $('#txtSalesLeadContactPersonEmail1').val();
    $('#txtDesignation1').val();
    $('#txtMobile11').val();
    $('#txtMobile21').val();
    $('#txtLinkedinurl1').val();
    $('#txtFirstName2').val();
    $('#txtLastName2').val();
    $('#txtSalesLeadContactPersonEmail2').val();
    $('#txtDesignation2').val();
    $('#txtMobile12').val();
    $('#txtMobile22').val();
    $('#txtLinkedinurl2').val()
}

function GetFilteredSalesLeadList() {

    ajaxCall("Get", false, '/SalesLead/SalesList', null, function (result) {



        if (result.status == true) {

            dataTable = $('#tblLeadContacts').DataTable({
                "responsive": true,
                "lengthChange": true,
                "paging": true,
                "searching": true,
                "processing": true, // for show progress bar
                /*"dom": 'Blfrtip',*/
                "filter": true, // this is for disable filter (search box)
                "data": result.data,
                "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],

                "columns": [
                    {
                        class: 'clsWrap',
                        data: "id",
                        title: 'Action',
                        name: "second",
                        render: function (data, type, row) {
                            if (data) {
                                return '<i class="fa fa-pen pen" value="' + data.id + '" onclick="AddEditSalesLead(' + row.id + ')"></i> | <i class="fa fa-trash trash btn-delete" value="' + data.id + '" onclick="DeleteSalesLead(' + row.id + ')"></i>';
                            }
                        }
                    },
                    { data: "orgName", title: "Lead", name: "first" },
                    {
                        data: null,
                        title: "Contact Person",
                        render: function (data) {
                            if (data.fullName == "Jigar Jadhav") {
                                $('td', data).css('background-color', 'Red');
                            }
                            return '<a href=/SalesLead/Actions?id=' + data.contactPersonId + '>' + data.fullName + ' </a> ';
                        }

                    },
                    { data: "mobile", title: "Mobile" },
                    { data: "email", title: "Email" },
                    { data: "designation", title: "Designation" }
                ],
                "rowsGroup": [
                    "first:name",
                    "second:name"

                ], "createdRow": function (row, data, dataIndex) {
                    
                    if (data.leadStatus == "DNC") {
                        $(row).addClass("table-danger");
                    }
                    else if (data.leadStatus == "Opportunity") {
                        $(row).addClass("table-success");
                    }
                }
            }).buttons().container().appendTo('#tblUser_wrapper .col-md-6:eq(0)');
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }

        $(".btn-edit").click(function (Id) {
            debugger
            EditMode = 1;
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

            ajaxCall("Post", false, '/SalesLead/DeleteSalesLead?Id=' + Id, null, function (result) {

                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    GetFilteredSalesLeadList();
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    })
};

function GetFilteredSalesConatactPersonList() {

    ajaxCall("Get", false, '/SalesLead/SalesConatactPersonList', null, function (result) {
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





