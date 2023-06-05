$(document).ready(function () {
    preventBack();
    noBack();
    applyRequiredValidation();
    $("#btnLogin").click(function () {
        var dataModel = {
            "UserName": $('#txtUserName').val(),
            "Password": $('#txtPassword').val()
        }
        console.log(dataModel);

        if (validateRequiredFields()) {
            $.blockUI({ message: "<h2>Please wait</p>" });

            ajaxCall("Post", false, '/LogIn/LogIn', JSON.stringify(dataModel), function (result) {

                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/Dashboard/Index");

                }
                else {
                    $.blockUI({
                        message: "<h2>Please wait</p>"
                    });
                    // setTimeout($.unblockUI, 2000);
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
            $.unblockUI();
        }
    });
    $("#btnForgotPassword").click(function () {
        var dataModel = {
            "Email": $('#txtEmail').val()
        }
        console.log(dataModel);
        if (validateRequiredFields()) {
            $.blockUI({
                message: "<h2>Please wait</p>"
            });
           // setTimeout($.unblockUI, 5000);
            ajaxCall("Post", false, '/LogIn/ForgotPassword', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {
                    console.log(result);
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/LogIn/LogIn");
                }
                else {
                    $.blockUI({
                        message: "<h2>Please wait</p>"
                    });
                    //setTimeout($.unblockUI, 5000);
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
            $.unblockUI();
        }
    });
    $("#btnResetPassword").click(function () {
        var dataModel = {
            "UserId": $('#txtUserId').val(),
            "NewPassword": $('#txtNewPassword').val()
        }
        console.log(dataModel);

        if (validateRequiredFields())
        {
            $.blockUI({
                message: "<h2>Please wait</p>"
            });
            //setTimeout($.unblockUI, 5000);
            ajaxCall("Post", false, '/LogIn/ResetPassword', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {
                    console.log(result);
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/LogIn/LogIn");
                }
                else {
                    $.blockUI({
                        message: "<h2>Please wait</p>"
                    });
                    //setTimeout($.unblockUI, 5000);
                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
            $.unblockUI();
        }
    });
    $("#btnChangePassword").click(function () {
        var dataModel = {
            "UserId": $('#txtUserId').val(),
            "OldPassword": $('#txtOldPassword').val(),
            "NewPassword": $('#txtNewPassword').val()
        }
        console.log(dataModel);

        if (validateRequiredFields()) {
            $.blockUI({
                message: "<h2>Please wait</p>"
            });
           // setTimeout($.unblockUI, 5000);
            ajaxCall("Post", false, '/LogIn/ChangePassword', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {
                    console.log(result);
                    Popup_Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/LogIn/LogIn");
                }
                else {
                    $.blockUI({
                        message: "<h2>Please wait</p>"
                    });
                   // setTimeout($.unblockUI, 2000);

                    //Toast.fire({ position: false, icon: 'error', title: result.message });
                    Popup_Toast.fire({ icon: 'error', title: result.message });
                    //Popup_Toast.fire({ icon: 'error', title: result.message, showConfirmButton: true });
                }
            });
            $.unblockUI();
        }
    });

});



function preventBack() { window.history.forward(); }
setTimeout("preventBack()", 0);
window.onunload = function () { null };
window.history.forward();
function noBack() {
    window.history.forward();
}






