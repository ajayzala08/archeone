$(document).ready(function () {
    LoadRequirementForDDL();
    LoadClientDDL();
    LoadPositionTypeDDL();
    LoadRequirementTypeDDL();
    LoadEmploymentTypeDDL();
    LoadRequirementStatusDDL();

    GetFilteredRequirementList();

    $("#btnAddRequirement").click(function () {
        AddEditRequirement(0);
    })

    $(".data-filter").change(function () {
        GetFilteredRequirementList();
    });

});

function GetFilteredRequirementList() {
    var reqData = {
        "RequirementForId": parseInt($("#ddlRequirementFor").val()),
        "ClientId": parseInt($("#ddlClients").val()),
        "PositionTypeId": parseInt($("#ddlPositionType").val()),
        "RequirementTypeId": parseInt($("#ddlRequirementType").val()),
        "EmploymentTypeId": parseInt($("#ddlEmploymentType").val()),
        "RequirementStatusId": parseInt($("#ddlRequirementStatus").val()),
    }
    ajaxCall("Get", false, '/Requirement/RequirementList', reqData, function (result) {
        $("#divRequirementList").html(result.responseText);
        ApplyDatatable('tblRequirement');
        //$(".ddl-status").each(function (index) {
        //    $(this).select2();
        //});
        $(".btn-resumes").click(function () {
            RedirectToPage('/UploadedResume/UploadedResume?RequirementId=' + $(this).attr('RequirementId'));
        });

        $(".btn-edit").click(function () {
            AddEditRequirement($(this).attr('RequirementId'));
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
                    var requirementId = parseInt($(this).attr('RequirementId'));
                    ajaxCall("Post", false, '/Requirement/DeleteRequirement?RequirementId=' + requirementId, null, function (result) {
                        if (result.status == true) {
                            Toast.fire({ icon: 'success', title: result.message });
                            RedirectToPage("/Requirement/Index");
                        }
                        else {
                            Toast.fire({ icon: 'error', title: result.message });
                            $.unblockUI();
                        }
                    });
                }
            });

        });

       // $(".ddl-status").select2();

        $(".ddl-status").change(function () {
            Swal.fire({
                title: 'Are you sure?',
                text: "You won't to change status!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, change it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.blockUI();
                    var requirementId = parseInt($(this).attr('requirementId'));
                    var requirementStatusId = parseInt($(this).val());
                    ajaxCall("Post", false, '/Requirement/ChangeStatus?RequirementId=' + requirementId + "&RequirementStatusId=" + requirementStatusId, null, function (result) {
                        if (result.status == true) {
                            Toast.fire({ icon: 'success', title: result.message });
                            RedirectToPage("/Requirement/Index");
                        }
                        else {
                            Toast.fire({ icon: 'error', title: result.message });
                            $.unblockUI();
                        }
                    });
                }
            });
        });
    });
}

function AddEditRequirement(RequirementId) {
    window.location.href = '/Requirement/AddEditRequirement?RequirementId=' + RequirementId;
}

function LoadRequirementForDDL() {
    ajaxCall("Get", false, '/Common/GetRequirementForList', null, function (result) {
        $("#ddlRequirementFor").html('');
        $("#ddlRequirementFor").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlRequirementFor").append('<option  value="' + value.id + '">' + value.requirementForName + '</option>');
            });
        }
       // $('#ddlRequirementFor').select2();
    });
}

function LoadClientDDL() {
    ajaxCall("Get", false, '/Common/GetClientList', null, function (result) {
        $("#ddlClients").html('');
        $("#ddlClients").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlClients").append('<option  value="' + value.id + '">' + value.clientName + '</option>');
            });
        }
       // $('#ddlClients').select2();
    });
}

function LoadPositionTypeDDL() {
    ajaxCall("Get", false, '/Common/GetPositionTypeList', null, function (result) {
        $("#ddlPositionType").html('');
        $("#ddlPositionType").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlPositionType").append('<option  value="' + value.id + '">' + value.positionTypeName + '</option>');
            });
        }
        //$('#ddlPositionType').select2();
    });
}

function LoadRequirementTypeDDL() {
    ajaxCall("Get", false, '/Common/GetRequirementTypeList', null, function (result) {
        $("#ddlRequirementType").html('');
        $("#ddlRequirementType").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlRequirementType").append('<option  value="' + value.id + '">' + value.requirementTypeName + '</option>');
            });
        }
       // $('#ddlRequirementType').select2();
    });
}

function LoadEmploymentTypeDDL() {
    ajaxCall("Get", false, '/Common/GetEmploymentTypeList', null, function (result) {
        $("#ddlEmploymentType").html('');
        $("#ddlEmploymentType").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlEmploymentType").append('<option  value="' + value.id + '">' + value.employmentTypeName + '</option>');
            });
        }
       // $('#ddlEmploymentType').select2();
    });
}

function LoadRequirementStatusDDL() {
    ajaxCall("Get", false, '/Common/GetRequirementStatusList', null, function (result) {
        $("#ddlRequirementStatus").html('');
        $("#ddlRequirementStatus").append('<option value="0"> Select All </option>');
        if (result.status == true) {
            $.each(result.data, function (index, value) {
                $("#ddlRequirementStatus").append('<option  value="' + value.id + '">' + value.requirementStatusName + '</option>');
            });
        }
        //$('#ddlRequirementStatus').select2();
    });
}