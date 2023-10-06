$(document).ready(function () {
    applyRequiredValidation();
    $("#btnChangeProfileImage").click(function () {
        if (validateRequiredFieldsByGroup("modal")) {
            loader_on();
            ChangeProfileImage();
        }
    });
});

function loadFile(event) {
    $('#lblProfileImage').html(event.target.files[0].name);
}


function ChangeProfileImage() {
    if (window.FormData !== undefined) {
        
        var saveData = new FormData();
        var file = $("#fileProfileImage").get(0).files[0];
        saveData.append("UserImage", file);
     
        if (validateRequiredFields()) {
            ajaxCallWithoutDataType("Post", false, '/Profile/ChangeProfileImage', saveData, function (result) {
               
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    $("#modalProfileImageUpload").hide();
                    window.location.reload();
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
    else {
        Toast.fire({ icon: 'error', title: "Please Select Profile Photo." });
    }
}
