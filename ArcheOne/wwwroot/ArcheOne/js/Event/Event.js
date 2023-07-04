/*var data;*/
$.ajax({
    type: 'GET',
    url: '/Event/EventData',
    cache: false,
    success: function (response) {
        debugger
        console.log(response);
        if (response.status == true) {
            showCalender(response.data);
        }
        else {
            debugger
            showCalender(response.data);
        }

    },
    
});

$(".close").click(function () {
    alert("Close event called");
    $("#myModal").modal("hide");

});



$("#addNewEvent").click(function () {
    alert("Add button click");
    var subject = $("#txtSubject").val();
    var description = $("#txtDescription").val();
    var start = $("#txtStart").val();
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

    $.ajax({
        type: 'POST',
        url: '/Event/AddEditEventData',
        contentType: 'application/json',
        data: JSON.stringify(addEvent),
        cache: false,
        success: function (response) {
            console.log(response);
            location.reload();
            showCalender(response.Data);

        }
    });

    $("#myModal").modal("hide");

});


function showCalender(data) {
    debugger
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
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
        window.location.href = '/Event/Event';
    });

});

function GetEventList() {
    ajaxCall("Get", false, '/Event/Event', null, function (result) {
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



