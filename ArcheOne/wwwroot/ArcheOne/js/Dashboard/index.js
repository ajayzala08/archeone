$(document).ready(function () {
    LoadBirthdayWorkAniversaryHoliday();

    showCalenderData();

    let hr = $("#txtHrId").val();
    let Sd = $("#txtsdId").val();
    let Sales = $("#txtSalesId").val();
    let Recruitment = $("#txtRecruitmentId").val();
    let QA = $("#txtqaId").val();
    let Designer = $("#txtDesignerId").val();
    
    if (hr) {
        ShowHRcharts();
    }
    else if (Sd || QA || Designer) {
        ShowSDcharts();
    } else if (Sales) {
        ShowSalescharts();
    } else if (Recruitment) {
        ShowHRcharts();
    }
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

            // change the border color just for fun
            info.el.style.borderColor = 'red';
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
                    startAngle:  -20,
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
