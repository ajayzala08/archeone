$(document).ready(function () {
    loader_on();
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
                        setTimeout(function () { window.location.reload() }, 3000); //window.location.href = window.location.href;
                    }
                    else {
                        Toast.fire({ icon: 'error', title: result.message });
                        setTimeout(function () { window.location.reload() }, 3000);
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
        loader_on();
        let salaryReqModel = {
            "CompanyId": parseInt($("#ddlCompany").val()),
            "SalaryYear": parseInt($("#ddlyear option:selected").text()),
            "SalaryMonth": $("#ddlmonth option:selected").text()
        }
        ajaxCall("Post", false, '/Salary/SearchSalary', JSON.stringify(salaryReqModel), function (result) {
            var IsDeletable = false;
            if (result.status == true) {

                IsDeletable = result.data.isDeletable;

                $('#tblSalary').DataTable({
                    "destroy": true,
                    "responsive": true,
                    "lengthChange": true,
                    "paging": true,
                    "processing": true,
                    "filter": true,
                    "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],

                    "data": result.data.salaryDetails,
                    "columns": [
                        {
                            class: 'clsWrap',
                            data: null,
                            title: 'Action',

                            render: function (data, type, row) {
                                var icons = '';
                                if (IsDeletable) {
                                    icons = '<i class="fa fa-trash trash" style="cursor: pointer;" value="' + data.salaryId + '" onclick="DeleteSalary(' + row.salaryId + ')"></i> | ';
                                }
                                icons += '<i class="fa fa-download btn-download" style="cursor: pointer;" value="' + data.salaryId + '" onclick="DownloadSalarySlip(' + row.salaryId + ')"></i>';
                                return icons;

                            }
                        },

                        { data: "employeeCode", title: "Employee Code" },
                        { data: "employeeName", title: "Employee Name" }
                    ]
                }).buttons().container().appendTo('#tblSalary_wrapper .col-md-6:eq(0)');;
                $.unblockUI();
            }
            else {
               
                Toast.fire({ icon: 'error', title: result.message });
                $.unblockUI();
              

            }

        });
        
    }
}
