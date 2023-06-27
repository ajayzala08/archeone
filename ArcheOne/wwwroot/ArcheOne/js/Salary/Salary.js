$(document).ready(function () {
    alert("JS loading....");
    $.blockUI();
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
        if (result.status == true) {
            
            $.each(result.data, function (data, value) {
                $("#ddlCompany").append($("<option></option>").val(value.id).html(value.companyName));
            })

        }
    });
}

function LoadYears() {
    ajaxCallWithoutDataType("GET", false, '/Salary/Years', null, function (result) {
        if (result.status == true) {
           
            $.each(result.data, function (data, value) {
                $("#ddlyear").append($("<option></option>").val(value.id).html(value.salaryYear));
            })

        }
    });
}

function LoadMonths() {
    ajaxCallWithoutDataType("GET", false, '/Salary/Months', null, function (result) {
        if (result.status == true) {
           
            $.each(result.data, function (data, value) {
                if (value.salaryMonth != "") {
                    $("#ddlmonth").append($("<option></option>").val(value.id).html(value.salaryMonth));
                }
            })

        }
    });
}


$("#btnSearch").click(function () {
    alert("Searching...");

    if (validateRequiredFieldsByGroup("modal")) {
        $.blockUI();
        let salaryReqModel = {
            "CompanyId": parseInt($("#ddlCompany").val()),
            "SalaryYear": parseInt($("#ddlyear option:selected").text()),
            "SalaryMonth": $("#ddlmonth option:selected").text()
        }
        alert(salaryReqModel);
        ajaxCall("Post", false, '/Salary/SearchSalary', JSON.stringify(salaryReqModel), function (result) {
            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
               
            } else {
                Toast.fire({ icon: 'error', title: result.message });
            }
            $.unblockUI();

            
        });
    }
    

});

$("#btnUploadSalarySheet").click(function () {
    if (validateRequiredFieldsByGroup("modelUpload")) {
        if (window.FormData !== undefined) {

            var saveData = new FormData();
            var file = $("#fileSalarySheet").get(0).files[0];
            saveData.append("SalarySheet", file);
            console.log(saveData);
                ajaxCallWithoutDataType("Post", false, '/Salary/UploadSalarySheet', saveData, function (result) {
                    console.log(result);
                    if (result.status == true) {
                        Toast.fire({ icon: 'success', title: result.message });
                        $("#modalSalaryUpload").hide();
                        window.location.reload();
                    }
                    else {
                        Toast.fire({ icon: 'error', title: result.message });
                        $.unblockUI();
                    }
                });
            
        }
        else {
            Toast.fire({ icon: 'error', title: "Please Select Profile Photo." });
        }
    }
});

