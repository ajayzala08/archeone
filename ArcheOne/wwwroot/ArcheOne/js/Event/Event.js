var data;
$.ajax({
    type: 'GET',
    url: '/Home/EventData',
    cache: false,
    success: function (response) {
        //alert(response);
        console.log(response);
        debugger;
        if (response.status == true) {
            //data = response;
            alert("response got");
            //$.each(data, function (key, value) {

            //    alert(data[key].title)
            //});
            showCalender(response.data);
        }
        else {
            alert(response.data);
        }

    },
    error: function () {
        alert("Error");
    }
});

$(".close").click(function () {
    alert("Close event called");
    $("#myModal").modal("hide");

});



$("#addNewEvent").click(function () {
    alert("Add button click");
    var subject = $("#txtSubject").val(); //prompt('Enter a date in YYYY-MM-DD format');
    var description = $("#txtDescription").val();
    var start = $("#txtStart").val(); // prompt('Enter a title');
    var end = $("#txtEnd").val();
    var theamColor = $('#ddlcolorId').val();
    var allDay = $('#chbIsFullDay').is(':checked');
    alert(subject + "-" + description + "-" + start + "-" + end + "-" + theamColor + "-" + allDay);

    var addEvent = {
        subject: $("#txtSubject").val(),
        description: $("#txtDescription").val(),
        start: $("#txtStart").val(),
        end: $("#txtEnd").val(),
        theamColor: $('#ddlcolorId').val(),
        isFullDay: $('#chbIsFullDay').is(':checked')

    };

    //calendar.addEvent(function(){
    //    title= subject,
    //    start= start,
    //    end= end,
    //    allDay= allDay,
    //    color= theamColor
    //});

    $.ajax({
        type: 'POST',
        url: '/Home/AddEventData',
        contentType: 'application/json',
        data: JSON.stringify(addEvent),
        cache: false,
        success: function (response) {
            debugger;
            console.log(response);
            location.reload();
            //alert("Refreshed called");
            showCalender(response.Data);
            //calendar.addEvent({
            //    title: subject,
            //    description:description,
            //    start: start,
            //    end: end,
            //    allDay: allDay,
            //    color: theamColor
            //});
        }
    });

    $("#myModal").modal("hide");

});


function showCalender(data) {
    debugger;
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        //dateClick: function () {
        //    alert('a day has been clicked!');
        //},
        headerToolbar: {
            left: 'prev,next,today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        footerToolbar: {
            start: 'custom1,custom2',
            center: '',
            end: 'prev,next'
        },
        customButtons: {
            custom1: {
                text: 'Add Event',
                click: function () {
                    $("#myModal").modal("show");
                   
                }
            },
            custom2: {
                text: 'custom 2',
                click: function () {
                    alert('clicked custom button 2!');
                }
            }
        },
        events: data,
     
        dateClick: function (info) {
            //var dateStr = prompt('Enter a date in YYYY-MM-DD format');
            //var titleStr = prompt('Enter a title');
            //var strDate = new Date(dateStr + 'T13:00:00'); // will be in local time
            //var endDate = new Date(dateStr + 'T15:00:00');
            //$("#myModal").modal("show");
            //if (!isNaN(strDate.valueOf()) && !isNaN(endDate.valueOf())) { // valid?
            //    calendar.addEvent({
            //        title: titleStr,
            //        start: strDate,
            //        end: endDate,
            //        allDay: false,
            //        color: 'purple'
            //    });
            //    alert('Great. Now, update your database...');
            //} else {
            //    alert('Invalid date.');
            //}
            //alert('Date: ' + info.dateStr);
            //alert('Resource ID: ' + info.view.title);
            //alert('DayEI', + info.dayEl)
        },
        eventClick: function (info) {

            alert('Event: ' + info.event.title);
            alert('Coordinates: ' + info.jsEvent.pageX + ',' + info.jsEvent.pageY);
            alert('View: ' + info.view.type);

            // change the border color just for fun
            info.el.style.borderColor = 'red';
        },
        editable: true,
        dayMaxEvents: true,
        navLinks: true, // can click day/week names to navigate views
        selectable: true,
        nowIndicator: true,
        now: '2023-06-02T02:45:00',

        click: function () {
          
        },
        eventDrop: function (info) {
            alert(info.event.title + " was dropped on " + info.event.start.toISOString());

            if (!confirm("Are you sure about this change?")) {
                info.revert();
            }
        }


    });

    calendar.render();
};


















$(document).ready(function () {
    GetEventList();
    $('#AddEvent').click(function () {
    /*    AddEditEvent(0);*/
        window.location.href = '/Event/EventList';
    });

});

function GetEventList() {
    ajaxCall("Get", false, '/Event/EventList', null, function (result) {
        $("#divEventList").html(result.responseText);
        ApplyDatatableResponsive('tblEvent');

        $(".btn-edit").click(function () {

            var Id = $(this).attr('Id');
            AddEditEvent(Id);
        });

        $(".btn-delete").click(function () {
            Id = $(this).attr('Id');
            DeleteEvent(Id);
        });

    });
}

function AddEditEvent(Id) {
    window.location.href = '/Event/AddEditEvent?Id=' + Id;
}

function DeleteEvent(Id) {
    if ($("#txtId").value > 0) {
        Id = $("#txtId").value;
    }
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

            ajaxCall("Post", false, '/Event/DeleteEvent?Id=' + Id, null, function (result) {
                if (result.status == true) {
                    Toast.fire({ icon: 'success', title: result.message });
                    GetFilteredHolidayList();
                }
                else {

                    Toast.fire({ icon: 'error', title: result.message });
                }
            });
        }

    })
};



