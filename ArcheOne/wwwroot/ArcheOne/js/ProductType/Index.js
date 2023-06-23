$(document).ready(function () {
    GetFilteredProductTypeList();

    $("#btnAddProductType").click(function () {
        AddEditProductType(0);
    })
});

function GetFilteredProductTypeList() {
    ajaxCall("Get", false, '/ProductType/ProductTypeList', null, function (result) {
        $("#divProductTypeList").html(result.responseText);
        ApplyDatatableResponsive('tblProductType');

        $(".btn-edit").click(function () {
            AddEditProductType($(this).attr('ProductTypeId'));
        });

        $(".btn-delete").click(function () {
            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.blockUI();
                    Toast.fire({ icon: 'success', title: result.message });
                    RedirectToPage("/ProductType/Index");
                    $.unblockUI();
                }
            });

        });
    });
}

function AddEditProductType(ProductTypeId) {
    RedirectToPage("/ProductType/AddEditProductType?ProductTypeId=" + ProductTypeId);
}
