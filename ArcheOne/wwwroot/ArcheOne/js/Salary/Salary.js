$(document).ready(function () {
    alert("JS loading....");
    LoadCompanies();
    LoadYears();
    LoadMonths();
    $.unblockUI();

});

function loadFile(event) {
    $('#lblSalarySheet').html(event.target.files[0].name);
}

function LoadCompanies() {
    ajaxCallWithoutDataType("GET", false, '/Salary/Companies', null, function (result) {
        console.log(result);
        if (result.status == true) {
            debugger
            $.each(result.data, function (data, value) {
                $("#ddlCompany").append($("<option></option>").val(value.id).html(value.companyName));
            })

        }
    });
}


function LoadYears() {
    ajaxCallWithoutDataType("GET", false, '/Salary/Years', null, function (result) {
        console.log(result);
        if (result.status == true) {
            debugger
            $.each(result.data, function (data, value) {
                $("#ddlyear").append($("<option></option>").val(value.id).html(value.salaryYear));
            })

        }
    });
}

function LoadMonths() {
    ajaxCallWithoutDataType("GET", false, '/Salary/Months', null, function (result) {
        console.log(result);
        if (result.status == true) {
            debugger
            $.each(result.data, function (data, value) {
                $("#ddlmonth").append($("<option></option>").val(value.id).html(value.salaryMonth));
            })

        }
    });
}