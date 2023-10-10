$(document).ready(function () {
    LoadBirthdayWorkAniversaryHoliday();

    showCalenderData();

    let hr = $("#txtHrId").val();
    let Sd = $("#txtsdId").val();
    let Sales = $("#txtSalesId").val();
    let Recruitment = $("#txtRecruitmentId").val();
    let QA = $("#txtqaId").val();
    let Designer = $("#txtDesignerId").val();
    let SuperAdmin = $("#txtSuperAdminId").val();
    let Admin = $("#txtISUserAdmin").val();

    if (hr) {
        ShowHRcharts();
    }
    else if (Sd || QA || Designer) {
        ShowSDcharts();
    } else if (Sales) {
        ShowSalescharts();
    } else if (Recruitment) {
        ShowHRcharts();
    } else if (SuperAdmin || Admin) {
        ShowHRcharts();
        ShowSDcharts();
        ShowSalescharts();
    }
    $("#Closemodel").click(function () {
        $("#CalenderPopup").hide();
    });
});

function LoadBirthdayWorkAniversaryHoliday() {
    ajaxCallWithoutDataType("GET", false, '/Dashboard/GetBirthdayWorkAniversaryHoliday', null, function (result) {
        if (result.status == true) {
            let holidayCnt = result.data.holidays.length;
            $('#holidaycnt').text(holidayCnt + ' Holidays');
            $.each(result.data.holidays, function (data, value) {
                let strToAdd = '<tr><td>' + value.holidayName + '</td><td>' + value.holidayDate + '</td></tr>';
                $('#tblHoliday').append(strToAdd);
            })

            let birthdayCnt = result.data.birthdays.length;
            $('#birthdayCnt').text(birthdayCnt + ' Birthday');
            $.each(result.data.birthdays, function (data, value) {
                let strToAdd = '<li><img src=' + value.employeeImagePath + ' alt="User Image" style="height:75px;width:75px;"></img><a class=users-list-name href=#>' + value.employeeName + ' </a> <span class=users-list-date>' + value.birthdate + '</span></li>';
                $('#libirthday').append(strToAdd);
            })

            let anniversaryCnt = result.data.workAnniversaries.length;
            $('#anniversaryCnt').text(anniversaryCnt + ' Anniversary');
            $.each(result.data.workAnniversaries, function (data, value) {
                let strToAdd = '<li><img src=' + value.employeeImagePath + ' alt="User Image" style="height:75px;width:75px;"></img><a class=users-list-name href=#>' + value.employeeName + ' </a> <span class=users-list-date>' + value.joinDate + '</span></li>';
                $('#liAnniversary').append(strToAdd);
            })
        }
    });
}

function showCalenderData() {

    $.ajax({
        type: 'GET',
        url: '/Event/EventData',
        cache: false,
        success: function (response) {
            //console.log(response);
            if (response.status == true) {
                showCalender(response.data);
            }
            else {
                showCalender(response.data);
            }

        },

    });
}
function showCalender(data) {

    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        themeSystem: 'bootstrap',

        contentHeight: 450,
        /*        headerToolbar: {
                    left: 'prev,next,today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek,timeGridDay'
                },
                footerToolbar: {
                    start: '',
                    center: '',
                    end: 'prev,next'
                },*/

        events: data,

        dateClick: function (info) {

        },

        eventClick: function (info) {

            showCalenderPopup(info.event._def.title);
            $("#CalenderPopup").show();

        },
        editable: false,
        dayMaxEvents: false,
        navLinks: false, // can click day/week names to navigate views
        selectable: false,
        nowIndicator: false,
        now: new Date(),
        /* now: '2023-06-02T02:45:00',*/

        click: function () {

        },
        eventDrop: function (info) {

            if (!confirm("Are you sure about this change?")) {
                info.revert();
            }
        }


    });

    calendar.render();
};

function ShowHRcharts() {

    ajaxCall("GET", false, '/Dashboard/HRChart', null, function (result) {
        var chartRequirmentContainer = new CanvasJS.Chart("chartRequirmentContainer", {
            title: {
                text: "Recruitment Chart"
            },
            animationEnabled: true,
            legend: {
                fontSize: 15,
                fontFamily: "Helvetica"
            },
            theme: "light2",
            data: [
                {
                    type: "doughnut",
                    //indexLabelFontFamily: "Garamond",
                    indexLabelFontSize: 20,
                    indexLabel: "{label} {y}",
                    startAngle: -20,
                    showInLegend: true,
                    toolTipContent: "{label} {y}",
                    dataPoints: result.data
                }],

        });
        chartRequirmentContainer.render();
    });
}

function ShowSDcharts() {
    ajaxCall("GET", false, '/Dashboard/SDChart', null, function (result) {
        var chartDeveloperContainer = new CanvasJS.Chart("chartDeveloperContainer", {
            title: {
                text: "Project Chart"
            },
            animationEnabled: true,
            legend: {
                fontSize: 15,
                fontFamily: "Helvetica"
            },
            theme: "light2",
            data: [
                {
                    type: "doughnut",
                    //indexLabelFontFamily: "Garamond",
                    indexLabelFontSize: 20,
                    indexLabel: "{label} {y}",
                    startAngle: -20,
                    showInLegend: true,
                    toolTipContent: "{label} {y}",
                    dataPoints: result.data
                }],

        });
        chartDeveloperContainer.render();
    });
}
function ShowSalescharts() {
    ajaxCall("GET", false, '/Dashboard/SalesChart', null, function (result) {
        var chartSalesContainer = new CanvasJS.Chart("chartSalesContainer", {
            title: {
                text: "Sales Chart"
            },
            animationEnabled: true,
            legend: {
                fontSize: 15,
                fontFamily: "Helvetica"
            },
            theme: "light2",
            data: [
                {
                    type: "doughnut",
                    //indexLabelFontFamily: "Garamond",
                    indexLabelFontSize: 20,
                    indexLabel: "{label} {y}",
                    startAngle: -20,
                    showInLegend: true,
                    toolTipContent: "{label} {y}",
                    dataPoints: result.data
                }],

        });
        chartSalesContainer.render();
    });
}
//function ShowRecruitmentcharts() {
//    ajaxCall("GET", false, '/Dashboard/RecruitmentChart', null, function (result) {
//        var chartRequirmentContainer = new CanvasJS.Chart("chartRequirmentContainer", {
//            title: {
//                text: "Recruitment Chart"
//            },
//            animationEnabled: true,
//            legend: {
//                fontSize: 15,
//                fontFamily: "Helvetica"
//            },
//            theme: "light2",
//            data: [
//                {
//                    type: "doughnut",
//                    indexLabelFontFamily: "Garamond",
//                    indexLabelFontSize: 15,
//                    indexLabel: "{label} {y}%",
//                    startAngle: -20,
//                    showInLegend: true,
//                    toolTipContent: "{label} {y}%",
//                    dataPoints: result.data
//                }],

//        });
//        chartRequirmentContainer.render();
//    });
//}
function showCalenderPopup(name) {

    ajaxCall("GET", false, '/Dashboard/CalenderPopupData?name=' + name, null, function (result) {
        if (result.status == true) {
            
            $("#EventTitle").text(result.data.title);
            $("#EventDescription").text(result.data.description);
            if (result.data.allDay) {
                $("#EventStartTime").text(result.data.start.split('T')[0] + ' ' + '(FullDayEvent)');
                $("#lableend:contains('EndDate')").hide();
              

            } else {
               
                var isoDate = new Date(result.data.start); // your ISO date

                var year = isoDate.getFullYear();
                var month = ("0" + (isoDate.getMonth() + 1)).slice(-2); // Months are zero based
                var date = ("0" + isoDate.getDate()).slice(-2);
                var hours = ("0" + isoDate.getHours()).slice(-2);
                var minutes = ("0" + isoDate.getMinutes()).slice(-2);

                var formattedDatestartDate = year + "-" + month + "-" + date + " " + hours + ":" + minutes;
                $("#lableend:contains('EndDate')").show();
                $("#EventStartTime").text(formattedDatestartDate);

                var isoDate1 = new Date(result.data.end); 
                var year1 = isoDate1.getFullYear();
                var month1 = ("0" + (isoDate1.getMonth() + 1)).slice(-2); // Months are zero based
                var date1 = ("0" + isoDate1.getDate()).slice(-2);
                var hours1 = ("0" + isoDate1.getHours()).slice(-2);
                var minutes1 = ("0" + isoDate1.getMinutes()).slice(-2);

                var formattedDateEndDate = year1 + "-" + month1 + "-" + date1 + " " + hours1 + ":" + minutes1;
                $("#EventEndTime").text(formattedDateEndDate);
              
            }
        }
        else {
            $("#CalenderPopup").hide();
        }
    });

}