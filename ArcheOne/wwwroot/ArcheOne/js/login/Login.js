$(document).ready(function () {
    applyRequiredValidation();
    $("#btnLogin").click(function () {
        var dataModel = {
            "UserName": $('#txtUserName').val(),
            "Password": $('#txtPassword').val()
        }
        console.log(dataModel);
        $.blockUI({message: "<h2>Please wait</p>"});
        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/LogIn', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/Dashboard/Index");
                    setTimeout($.unblockUI, 2000);
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    setTimeout($.unblockUI, 2000);
                }
            });
        }
    });

    $("#btnForgotPassword").click(function () {
        var dataModel = {
            "Email": $('#txtEmail').val()
        }
        console.log(dataModel);
        $.blockUI({message: "<h2>Please wait</p>" });
        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/ForgotPassword', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {
                    console.log(result);
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/LogIn/LogIn");
                    setTimeout($.unblockUI, 2000);
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    setTimeout($.unblockUI, 2000);
                }
            });
        }
    });

    $("#btnResetPassword").click(function () {
        var dataModel = {
            "UserId": $('#txtUserId').val(),
            "NewPassword": $('#txtNewPassword').val()
        }
        console.log(dataModel);
        $.blockUI({ message: "<h2>Please wait</p>"});
        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/ResetPassword', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {
                    console.log(result);
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/LogIn/LogIn");
                    setTimeout($.unblockUI, 2000);
                }
                else {
                    Toast.fire({ icon: 'error', title: result.message });
                    setTimeout($.unblockUI, 2000);
                }
            });
           
        }
    });

    $("#btnChangePassword").click(function () {
        var dataModel = {
            "UserId": $('#txtUserId').val(),
            "OldPassword": $('#txtOldPassword').val(),
            "NewPassword": $('#txtNewPassword').val()
        }
        console.log(dataModel);
        $.blockUI({message: "<h2>Please wait</p>" });
        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/ChangePassword', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {
                    console.log(result);
                    Popup_Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/LogIn/LogIn");
                    setTimeout($.unblockUI, 2000);
                }
                else {
                    Popup_Toast.fire({ icon: 'error', title: result.message });
                    //Popup_Toast.fire({ icon: 'error', title: result.message, showConfirmButton: true });
                    setTimeout($.unblockUI, 2000);
                }
            });
        }
    });

    $("#BtnLogout").click(function () {
    ajaxCall("get", false, '/LogIn/Logout', '', function (result) {
        if (result.status == true) {
            console.log(result);
            // Popup_Toast.fire({ icon: 'success', title: result.message });
            RedirectToPage("/LogIn/LogIn");
            //preventBack();
            //noBack();
        }
        else {
        }
      
    });
    

    });
});



function preventBack() { window.history.forward(); }
setTimeout("preventBack()", 0);
window.onunload = function () { null };
window.history.forward();
function noBack() {
    window.history.forward();
}






