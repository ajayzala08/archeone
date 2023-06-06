var EditMode = 1;
$(document).ready(function () {
    GetFilteredOrganization();
    $("#btnAddSaleLead").click(function () {
        AddEditOrganization(0);
    });
});
