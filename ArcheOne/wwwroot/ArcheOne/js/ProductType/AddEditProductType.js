$(document).ready(function () {
    applyRequiredValidation();
    $("#btnSaveUpdateProductType").click(function () {
        if (validateRequiredFields()) {
            //$.blockUI();
            Toast.fire({ icon: 'success', title: "Data saved successfully!" });
            //RedirectToPage("/ProductType/Index");
        }
    });

    $("#btnCancelProductType").click(function () {
        RedirectToPage("/ProductType/Index");
    });
});

