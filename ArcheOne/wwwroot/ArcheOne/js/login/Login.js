$(document).ready(function () {
    $("#btnLogin").click(function () {
        $.blockUI({
            css: {
                position: 'fixed',
                margin: 'auto',
                border: 'none',
                backgroundColor: 'none',
                color: '#fff',
                fontSize: '20px',
                'text-align': 'center',
                padding: '0px',
                width: '223px',
                top: '41%',
                left: '41%',
                height: '109px',
                fontsize: '20px'
            }, message: 'Please Wait...'
        });
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
        $.blockUI();
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
        $.blockUI();
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
        $.blockUI();
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




function runScript(e) {
    if (e.keyCode == 13) {

        $("#btnLogin").click();

        return false;
    }
}




