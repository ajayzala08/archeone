$(document).ready(function () {
    setTimeout("preventBack()", 0);
    window.onunload = function () { null };
    $("#btnLogin").click(function () {
        loader_on();
        var dataModel = {
            "UserName": $('#txtUserName').val(),
            "Password": $('#txtPassword').val(),
            "RememberMe": $('#rememberMeCheckbox').is(':checked')
        }
        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/LogIn', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {
                    RedirectToPage("/Dashboard/Index");
                    Toast.fire({ icon: 'success', title: result.message });
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    $.unblockUI();
                }
            });
        }
        else {
            $.unblockUI();
        }
    });

    $("#btnForgotPassword").click(function () {
        loader_on();
        var dataModel = {
            "Email": $('#txtEmail').val()
        }

        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/ForgotPassword', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {

                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/LogIn/LogIn");
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    $.unblockUI();
                }

            });
        }
        else {
            $.unblockUI();
        }
    });

    $("#btnResetPassword").click(function () {
        loader_on();
        var dataModel = {
            "UserId": $('#txtUserId').val(),
            "NewPassword": $('#txtNewPassword').val()
        }
        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/ResetPassword', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {

                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/LogIn/LogIn");
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    $.unblockUI();
                }
            });
        }
        else {
            $.unblockUI();
        }
    });

    $("#btnChangePassword").click(function () {
        loader_on();
        var dataModel = {
            "UserId": $('#txtUserId').val(),
            "OldPassword": $('#txtOldPassword').val(),
            "NewPassword": $('#txtNewPassword').val()
        }
        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/ChangePassword', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {

                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/LogIn/LogIn");
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    $.unblockUI();
                }
            });
        }
        else {
            $.unblockUI();
        }
    });

    $("#btnCancelCP").click(function () {
        window.location.href = '/Dashboard/Index';
    });
});


function preventBack() { window.history.forward(); }

function runScript(e) {
    if (e.keyCode == 13) {

        $("#btnLogin").click();

        return false;
    }
}




