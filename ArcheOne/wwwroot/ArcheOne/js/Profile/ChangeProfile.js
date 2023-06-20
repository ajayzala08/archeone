$(document).ready(function () {
    applyRequiredValidation();
    $("#btnChangeProfileImage").click(function () {
        ChangeProfileImage(); 
    });
});

function ChangeProfileImage() {
    if (validateRequiredFieldsByGroup("dtxtResumeUpload")) {
        alert("Change button clicked");
    }
}

function loadFile(event) {
    $('#lblResumeUpload').html(event.target.files[0].name);
}