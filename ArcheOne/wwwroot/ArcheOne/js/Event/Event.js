/*var data;*/
$.ajax({
    type: 'GET',
    url: '/Event/EventData',
    cache: false,
    success: function (response) {
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
    $("#myModal").modal("hide");

});



$("#addNewEvent").click(function () {
   
    var subject = $("#txtSubject").val();
    var description = $("#txtDescription").val();
    var start = $("#txtStartDate").val();
    var end = $("#txtEndDate").val();
    var theamColor = $('#ddlcolorId').val();
    var allDay = $('#chbIsFullDay').is(':checked');

    var addEvent = {
        subject: $("#txtSubject").val(),
        description: $("#txtDescription").val(),
        start: $("#txtStartDate").val(),
        end: $("#txtEndDate").val(),
        theamColor: $('#ddlcolorId').val(),
        isFullDay: $('#chbIsFullDay').is(':checked')

    };
    if (validateRequiredFields()) {
        $.ajax({
            type: 'POST',
            url: '/Event/AddEditEventData',
            contentType: 'application/json',
            data: JSON.stringify(addEvent),
            cache: false,
            success: function (response) {
                
                if (response.status == true) {
                
                    Toast.fire({ icon: 'success', title: response.message });
                    console.log(response);
                    location.reload();
                    showCalender(response.Data);

                }
                else {
                    Toast.fire({ icon: 'error', title: response.message });
                    $.unblockUI();
                }

            }
        });

        $("#myModal").modal("hide");

    }

});

$("#btnCancel").click(function () {
    window.location.href = '/Event/Event';
});
function showCalender(data) {
  
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next,today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        footerToolbar: {
            start: 'custom1',
            center: '',
            end: 'prev,next'
        },
        customButtons: {
            custom1: {
                text: 'Add Event',
                click: function () {
                    $("#myModal").modal("show");

                }
            }
            
        },
        events: data,

        dateClick: function (info) {
            
        },
        eventClick: function (info) {

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



