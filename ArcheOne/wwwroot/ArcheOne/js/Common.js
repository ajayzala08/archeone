﻿var methodType = "Post";
var baseURL = "http://localhost:802/";
var DataType = "json";
var isDataTypeJson = false;

var Toast = Swal.mixin({
    toast: true,
    //showCancelButton: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 5000
});

var Popup_Toast = Swal.mixin({
    position: 'center',
    //icon: 'success',
    //title: 'Your work has been saved',
    showConfirmButton: false,
    timer: 5000
})

$(document).ready(function () {
    ApplyEvents();
});

// General function for all ajax calls
function ajaxCall(methodType, applyBaseURL, apiURL, dataParams, callback) {
    //var Token = $("#txtToken").val();
    var URL;
    if (applyBaseURL == true) {
        URL = baseURL + apiURL;
    }
    else {
        URL = apiURL;
    }

    $.ajax({
        type: methodType,
        url: URL,
        //quietMillis: 100,
        headers: {
            //'Authorization': Token,
            //"Content-Type": "application/json"
            /* 'Access-Control-Allow-Origin': '*'*/
        },
        contentType: 'application/json; charset=utf-8',
        data: dataParams,
        dataType: DataType,
        //cache: false,
        success: function (response) {
            callback(response);
        },
        error: function (response) {
            callback(response);
        }
    });
}

// General function for all ajax calls
function ajaxCallWithoutDataType(methodType, applyBaseURL, apiURL, dataParams, callback) {
    //var Token = $("#txtToken").val();
    var URL;
    if (applyBaseURL == true) {
        URL = baseURL + apiURL;
    }
    else {
        URL = apiURL;
    }

    $.ajax({
        type: methodType,
        url: URL,
        //quietMillis: 100,
        headers: {
            //'Authorization': Token,
            //"Content-Type": "application/json"
            /* 'Access-Control-Allow-Origin': '*'*/
        },
        contentType: false, // Not to set any content header  
        processData: false, // Not to process data  
        data: dataParams,
        /* dataType: DataType,*/
        //cache: false,
        success: function (response) {
            callback(response);
        },
        error: function (response) {
            callback(response);
        }
    });
}


function RedirectToPage(path) {
    window.setTimeout(function () {
        window.location = path;
    }, 1000);
}

function ApplyDatatable(id) {
    datatableId = "#" + id;
    datatableWrapper = datatableId + "_wrapper";
    $(datatableId).DataTable({
        "responsive": true,
        "lengthChange": false,
        "autoWidth": true,
        //"scrollX" : true,
        "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"]
    }).buttons().container().appendTo(datatableWrapper + ' .col-md-6:eq(0)');
}

function ApplyEvents() {
    $(".onlyNumeric").bind("paste", function () {
        onlyNumbers(event);
    });
    $(".onlyNumeric").keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode === 67 && (e.ctrlKey === true || e.metaKey === true)) ||
            (e.keyCode === 86 && (e.ctrlKey === true || e.metaKey === true)) ||
            //Allow : Ctrl+X
            (e.keyCode === 88 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    $('.onlyletter').keydown(function (e) {
        if (e.altKey) {
            e.preventDefault();
        }
        else {
            var key = e.keyCode;
            if (!((key == 8) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90) || (key == 9))) {
                e.preventDefault();
            }
        }
    });
    $('.txtCapitalize').blur(function () {
        var str = $(this).val();
        var spart = str.split(" ");
        for (var i = 0; i < spart.length; i++) {
            var j = spart[i].charAt(0).toUpperCase();
            spart[i] = j + spart[i].substr(1);
        }
        $(this).val(spart.join(" "));
    });
    //$(".inputField").on("input", function () {
    //    var input = $(this).val();
    //    if (input.length != 1) {
    //        alert("Only one letter allowed!");
    //        $(this).val("");
    //    }
    //});

    $(".text-length").blur(function () {
        var obj = $(this);
        var val = $(obj).val().trim();
        if (val.length < $(obj).attr("minlength") || val.length > $(obj).attr("maxlength")) {
            var min = parseInt($(obj).attr("minlength"));
            var max = parseInt($(obj).attr("maxlength"));
            $("#" + $(obj).attr("errorspan")).removeClass('d-none');
            $("#" + $(obj).attr("divcontainer")).addClass('has-error');
            $("#" + $(obj).attr("id")).addClass('is-invalid');
        }
        else {
            $("#" + $(obj).attr("errorspan")).addClass('d-none');
            $("#" + $(obj).attr("divcontainer")).removeClass('has-error');
            $("#" + $(obj).attr("id")).removeClass('is-invalid');
        }
    });
}

function validateRequiredFields() {
    $('.error').each(function (index, itm) {
        if (!$(itm).hasClass('d-none')) {
            $(itm).addClass('d-none');
        }
    });
    $('.has-error').each(function (index, itm) {
        if ($(this).hasClass('has-error')) {
            $(this).removeClass('has-error');
        }
    });

    $("[isRequired='1']").each(function (ind, item) {
        validateReqField($(this));
    });

    if ($('.has-error').length > 0) {
        $($($('.has-error').first()).find("input[isrequired='1']").first()).focus();
        return false;
    }
    return true;
}

function applyRequiredValidation() {
    $("[isRequired='1']").each(function (ind, item) {
        $(this).change(function () {
            validateReqField($(this));
        });
        $(this).blur(function () {
            validateReqField($(this));
        })
    });
}

function validateReqField(obj) {
    if (Array.isArray($(obj).val()) && $(obj).val().length === 0) {
        $("#" + $(obj).attr("errorspan")).removeClass('d-none');
        $("#" + $(obj).attr("divcontainer")).addClass('has-error');
        $("#" + $(obj).attr("id")).addClass('is-invalid');
    }
    else if (!Array.isArray($(obj).val()) && $(obj).val().trim() == $(obj).attr("defaultvalue")) {
        $("#" + $(obj).attr("errorspan")).removeClass('d-none');
        $("#" + $(obj).attr("divcontainer")).addClass('has-error');
        $("#" + $(obj).attr("id")).addClass('is-invalid');
    }
    else {
        $("#" + $(obj).attr("errorspan")).addClass('d-none');
        $("#" + $(obj).attr("divcontainer")).removeClass('has-error');
        $("#" + $(obj).attr("id")).removeClass('is-invalid');
    }
    if ($(obj).val().length < $(obj).attr("minlength") || $(obj).val().length > $(obj).attr("maxlength")) {
        var min = parseInt($(obj).attr("minlength"));
        var max = parseInt($(obj).attr("maxlength"));
        //console.log("min", $(obj).attr("minlength"));
        //console.log("max", $(obj).attr("maxlength"));
        // console.log("len", $(obj).val().length);

        $("#" + $(obj).attr("errorspan")).removeClass('d-none');
        $("#" + $(obj).attr("divcontainer")).addClass('has-error');
        $("#" + $(obj).attr("id")).addClass('is-invalid');

        //if (min > 0 && max > 0 && min == max && min != $(obj).val().length) {
        //    $("#" + $(obj).attr("errorspan")).removeClass('d-none');
        //    $("#" + $(obj).attr("divcontainer")).addClass('has-error');
        //}
        //else if (min > 0 && max > 0 && min != max && !(min <= $(obj).val().length && max >= $(obj).val().length)) {
        //    $("#" + $(obj).attr("errorspan")).removeClass('d-none');
        //    $("#" + $(obj).attr("divcontainer")).addClass('has-error');
        //}
        //else {
        //    $("#" + $(obj).attr("errorspan")).addClass('d-none');
        //    $("#" + $(obj).attr("divcontainer")).removeClass('has-error');
        //}
    }
    //else {
    //    $("#" + $(obj).attr("errorspan")).addClass('d-none');
    //    $("#" + $(obj).attr("divcontainer")).removeClass('has-error');
    //    $("#" + $(obj).attr("id")).removeClass('is-invalid');
    //}
}

function GetFormattedDateString(date) {
    var d = new Date(date);
    var day = d.getDate();
    var month = d.getMonth() + 1;
    var year = d.getFullYear();
    if (day < 10) {
        day = "0" + day;
    }
    if (month < 10) {
        month = "0" + month;
    }
    var date = day + "/" + month + "/" + year;
    return date;
}

function ApplyDatatableResponsive(id) {
    datatableId = "#" + id;
    datatableWrapper = datatableId + "_wrapper";
    $(datatableId).DataTable({
        "responsive": false,
        //"lengthChange": true,
        //"autoWidth": true,
        //"scrollX": true,
        "initComplete": function (settings, json) {
            $(datatableId).wrap("<div style='overflow:auto; width:100%;position:relative;'></div>");
        },
        "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"]
    }).buttons().container().appendTo(datatableWrapper + ' .col-md-6:eq(0)');
}

function getDateFromFields(fieldId) {
    var dtParts = $("#" + fieldId).val().split('/');
    var monthName = "";
    switch (dtParts[1]) {
        case "02":
            monthName = "FEB";
            break;
        case "03":
            monthName = "MAR";
            break;
        case "04":
            monthName = "APR";
            break;
        case "05":
            monthName = "MAY";
            break;
        case "06":
            monthName = "JUN";
            break;
        case "07":
            monthName = "JUL";
            break;
        case "08":
            monthName = "AUG";
            break;
        case "09":
            monthName = "SEP";
            break;
        case "10":
            monthName = "OCT";
            break;
        case "11":
            monthName = "NOV";
            break;
        case "12":
            monthName = "DEC";
            break;
        default:
            monthName = "JAN";
            break;
    }

    return dtParts[0] + "/" + monthName + "/" + dtParts[2];
}

function validateRequiredFieldsByGroup(groupId) {
    $('[id="' + groupId + '"] .error').each(function (index, itm) {
        if (!$(itm).hasClass('d-none')) {
            $(itm).addClass('d-none');
        }
    });
    $('[id="' + groupId + '"] .has-error').each(function (index, itm) {
        if ($(this).hasClass('has-error')) {
            $(this).removeClass('has-error');
        }
    });
    $("[id='" + groupId + "'] [isRequired='1']").each(function (ind, item) {
        validateReqField($(this));
    });
    if ($('[id="' + groupId + '"] .has-error').length > 0) {
        return false;
    }
    return true;
}
function ApplyDatatableForPolicy(id) {
    datatableId = "#" + id;
    datatableWrapper = datatableId + "_wrapper";
    $(datatableId).DataTable({
        "responsive": true,
        "lengthChange": false,
        "autoWidth": false,
        //"scrollX" : true,
        // "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"]
    }).buttons().container().appendTo(datatableWrapper + ' .col-md-6:eq(0)');
}

function onlyNumbers(event) {
    debugger
    if (event.type == "paste") {
        var clipboardData = event.clipboardData || window.clipboardData;
        var pastedData = clipboardData.getData('Text');
        if (isNaN(pastedData)) {
            event.preventDefault();

        } else {
            return;
        }
    }
}