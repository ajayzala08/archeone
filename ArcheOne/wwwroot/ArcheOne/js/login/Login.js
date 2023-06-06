$(document).ready(function () {

    //applyRequiredValidation();
    $("#btnLogin").click(function () {
        $.blockUI();
        var dataModel = {
            "UserName": $('#txtUserName').val(),
            "Password": $('#txtPassword').val(),
            "RememberMe": $('#rememberMeCheckbox').is(':checked')
        }
        /*console.log(dataModel);*/
        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/LogIn', JSON.stringify(dataModel), function (result) {
                //console.log(result);
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
        console.log(dataModel);
        if (validateRequiredFields()) {
            ajaxCall("Post", false, '/LogIn/ForgotPassword', JSON.stringify(dataModel), function (result) {
                if (result.status == true) {
                    console.log(result);
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
                    console.log(result);
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
                    console.log(result);
                    Popup_Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/LogIn/LogIn");
                }
                else {
                    Popup_Toast.fire({ icon: 'error', title: result.message });
                    $.unblockUI();
                }
            });
        }
        else {
            $.unblockUI();
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

function getCookie(name) {
    // var cookieName = encodeURIComponent(name) + '=';
    var cookieName = encodeURIComponent(name);
    var cookieArray = document.cookie.split(';');

    for (var i = 0; i < cookieArray.length; i++) {
        var cookie = cookieArray[i].trim(); // Trim any leading/trailing whitespace

        if (cookie.indexOf(cookieName) === 0) {
            return decodeURIComponent(cookie.substring(cookieName.length));
        }
    }

    return null;
}





