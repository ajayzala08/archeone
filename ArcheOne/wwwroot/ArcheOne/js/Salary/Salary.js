$(document).ready(function () {
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

    SalaryDataFill();
    

});

$("#btnUploadSalarySheet").click(function () {
    if (validateRequiredFieldsByGroup("modelUpload")) {
        
        if (window.FormData !== undefined) {
            var saveData = new FormData();
            var file = $("#fileSalarySheet").get(0).files[0];
            saveData.append("SalarySheet", file);
                ajaxCallWithoutDataType("Post", false, '/Salary/UploadSalarySheet', saveData, function (result) {
                    if (result.status == true) {
                        Toast.fire({ icon: 'success', title: result.message });
                        window.location.reload(); //window.location.href = window.location.href;
                    }
                    else {
                        Toast.fire({ icon: 'error', title: result.message });
                        
                    }
                });
            
        }
        else {
            Toast.fire({ icon: 'error', title: "Please Select Salarysheet" });
        }
        
    }
});


function DeleteSalary(SalaryId) {
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
            ajaxCall("Post", false, '/Salary/DeleteSalary?id=' + SalaryId, null, function (result) {
                if (result.status == true) {
                    SalaryDataFill();
                    Toast.fire({ icon: 'success', title: result.message });
                    
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    })

}

function DownloadSalarySlip(salaryId) {

    window.open('/Salary/DownloadSalarySlip?id=' + salaryId);
}


function SalaryDataFill() {
    if (validateRequiredFieldsByGroup("modal")) {
        $.blockUI();
        let salaryReqModel = {
            "CompanyId": parseInt($("#ddlCompany").val()),
            "SalaryYear": parseInt($("#ddlyear option:selected").text()),
            "SalaryMonth": $("#ddlmonth option:selected").text()
        }
        ajaxCall("Post", false, '/Salary/SearchSalary', JSON.stringify(salaryReqModel), function (result) {
            if (result.status == true) {
                $('#tblSalary').DataTable({
                    "destroy": true,
                    "responsive": true,
                    "lengthChange": true,
                    "paging": true,
                    "searching": true,
                    "processing": true, // for show progress bar
                    "dom": 'Blfrtip',
                    // "retrieve": true,
                    "filter": true, // this is for disable filter (search box)
                    "data": result.data,
                    "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],
                    "columns": [
                        {
                            class: 'clsWrap',
                            data: null,
                            title: 'Action',

                            render: function (data, type, row) {

                                return '<i class="fa fa-trash trash" value="' + data.salaryId + '" onclick="DeleteSalary(' + row.salaryId + ')"></i> | <i class="fa fa-download btn-download" value="' + data.salaryId + '" onclick="DownloadSalarySlip(' + row.salaryId + ')"></i>';

                            }
                        },

                        { data: "employeeCode", title: "Employee Code" },
                        { data: "employeeName", title: "Employee Name" }
                    ]
                });
                $.unblockUI();
            }
            else {
               
                Toast.fire({ icon: 'error', title: result.message });
                $.unblockUI();
              

            }

        });
        
    }
}
