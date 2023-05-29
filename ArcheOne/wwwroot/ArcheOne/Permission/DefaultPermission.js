$(document).ready(function () {
    //applyRequiredValidation();
    $("#btnLogin").click(function () {
        var dataModel = {
            "UserName": $('#txtUserName').val(),
            "Password": $('#txtPassword').val()
        }
        console.log(dataModel);
        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/LogIn', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/Dashboard/Index");
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }
    });
});

function LoadDefaultPermissions() {

}