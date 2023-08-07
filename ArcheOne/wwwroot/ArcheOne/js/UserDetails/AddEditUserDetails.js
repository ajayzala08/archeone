$(document).ready(function () {
    applyRequiredValidation();
    $("#btnSaveUpdateUserDetails").click(function () {
        SaveUserDetails();
    });
});
var minDate = new Date();
//minDate.setFullYear(minDate.getFullYear() - 18);

//$('#txtDob').datepicker({
//    dateFormat: 'dd-mm-yy',
//    maxDate: minDate


//});

var dtToday = new Date();
var month = dtToday.getMonth() + 1;// jan=0; feb=1 .......
var day = dtToday.getDate();
var year = dtToday.getFullYear() - 18;
if (month < 10)
    month = '0' + month.toString();
if (day < 10)
    day = '0' + day.toString();
var minDate1 = year + '-' + month + '-' + day;
var maxDate1 = year + '-' + month + '-' + day;
$('#txtDob').attr('max', maxDate1);
//$('#txtDob').val(maxDate1);


/*const [today] = new Date().toISOString().split('T');*/
const maxDate = new Date();
maxDate.setDate(maxDate.getDate() - 30);
const [maxDateFormatted] = maxDate.toISOString().split('T');
const dateInput = document.getElementById('txtOfferDate');
dateInput.setAttribute('min', maxDateFormatted);
//$('#txtOfferDate').val(maxDateFormatted);



const maxDateJoin = new Date();
maxDateJoin.setDate(maxDateJoin.getDate() - 30);
const [maxDateFormatted1] = maxDateJoin.toISOString().split('T');
const dateInput1 = document.getElementById('txtJoinDate');
dateInput1.setAttribute('min', maxDateFormatted1);
//$('#txtJoinDate').val(maxDateFormatted1);
function SaveUserDetails() {
    var saveData = new FormData();
    saveData.append("Id", parseInt($("#txtId").val()));
    saveData.append("UserId", parseInt($("#txtUserId").val()));
    saveData.append("EmployeeCode", $("#txtEmployeeCode").val());
    saveData.append("Gender", $("#ddlGender").val());
    saveData.append("EmergencyContact", $("#txtEmergencyContact").val());
    saveData.append("Dob", $("#txtDob").val());
    saveData.append("PostCode", $("#txtPostCode").val());
    saveData.append("EmploymentType", $("#ddlEmploymentType").val());
    saveData.append("Location", $("#txtLocation").val());
    saveData.append("BloodGroup", $("#txtBloodGroup").val());
    saveData.append("OfferDate", $("#txtOfferDate").val());
    saveData.append("JoinDate", $("#txtJoinDate").val());
    saveData.append("BankName", $("#txtBankName").val());
    saveData.append("AccountNumber", $("#txtAccountNumber").val());
    saveData.append("Branch", $("#txtBranch").val());
    saveData.append("IfscCode", $("#txtIfscCode").val());
    saveData.append("PfaccountNumber", $("#txtPfaccountNumber").val());
    saveData.append("PancardNumber", $("#txtPancardNumber").val());
    saveData.append("AdharCardNumber", $("#txtAdharCardNumber").val());
    saveData.append("Salary", $("#txtSalary").val());
    saveData.append("ReportingManager", $("#ddlReportingManager").val());
    saveData.append("EmployeePersonalEmailId", $("#txtEmployeePersonalEmailId").val());
    saveData.append("ProbationPeriod", $("#txtProbationPeriod").val());
    
    if (validateRequiredFields()) {
        ajaxCallWithoutDataType("Post", false, '/UserDetails/SaveUpdateUserDetails', saveData, function (result) {
            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
                RedirectToPage("/User/User");
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
                $.unblockUI();
            }
        });
    }
    else {
        $.unblockUI();
        Toast.fire({ icon: 'success', title: result.message });
        $("#clearAll").click();
        ClearAll();
    }
}

$("#txtEmployeeCode").blur(function () {

    const urlParams = new URLSearchParams(window.location.search);
    var userId = urlParams.get('userId');
    if ($("#txtEmployeeCode").val() != "") {
        var checkEmployeeCodeReqModel = {
            "Id": parseInt(userId),
            "EmployeeCode": parseInt($("#txtEmployeeCode").val())
        }
        ajaxCall("Post", false, '/UserDetails/CheckEmployeeCode', JSON.stringify(checkEmployeeCodeReqModel), function (result) {

            if (result.status == true) {
                Toast.fire({ icon: 'success', title: result.message });
            }
            else {
                Toast.fire({ icon: 'error', title: result.message });
                $("#txtEmployeeCode").val("");
                $("#txtEmployeeCode").focus();
            }
        });
    }

});

$("#btnClose").click(function () {
    RedirectToPage("/User/User");
});

$("#txtDob").blur(function () {
    var startDate = new Date($("#txtDob").val());;
    var endDate = minDate;

    if (startDate > endDate) {
        $("#txtDob").val("");
    }

});

$("#txtEmployeeCode").keypress(function (e) {
    if (String.fromCharCode(e.keyCode).match(/[^0-9]/g)) return false;

    if ($(this).val().length >= 10) return false;
});

function keyPress(event) {
    event.preventDefault();
    return false;
}