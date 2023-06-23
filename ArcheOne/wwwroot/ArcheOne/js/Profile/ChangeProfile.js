$(document).ready(function () {
    applyRequiredValidation();
    $("#btnChangeProfileImage").click(function () {
        if (validateRequiredFieldsByGroup("modal")) {
            $.blockUI();
            ChangeProfileImage();
        }
    });
});

function loadFile(event) {
    $('#lblResumeUpload').html(event.target.files[0].name);
}


function ChangeProfileImage() {
    if (window.FormData !== undefined) {
        
        var saveData = new FormData();
        var file = $("#fileProfileImage").get(0).files[0];
        saveData.append("UserImage", file);
        console.log(saveData);
        if (validateRequiredFields()) {
            ajaxCallWithoutDataType("Post", false, '/Profile/ChangeProfileImage', saveData, function (result) {
                console.log(result);
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
