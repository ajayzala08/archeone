/*var data;*/
$.ajax({
    type: 'GET',
    url: '/Event/EventData',
    cache: false,
    success: function (response) {
        
        if (response.status == true) {
            showCalender(response.data);
        }
        else {
            showCalender(response.data);
        }

    },
    
});

$(".close").click(function () {
    $("#myModal").modal("hide");

});

$("#Closemodel").click(function () {
    $("#CalenderPopup").hide();
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
        themeSystem: 'bootstrap',
      
        headerToolbar: {
            left: 'prev,next,today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        footerToolbar: {
            start: '',
            center: '',
            end: 'prev,next'
        },
     
        events: data,

        dateClick: function (info) {
            
        },
        eventClick: function (info) {

            // change the border color just for fun
            // info.el.style.borderColor = 'red';
            
                showCalenderPopup(info.event._def.title);
                $("#CalenderPopup").show();
        },
        editable: true,
        dayMaxEvents: true,
        navLinks: true, // can click day/week names to navigate views
        selectable: true,
        nowIndicator: true,
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

$(document).ready(function () {
    GetEventList();
    //$('#AddEvent').click(function () {
       
    //    window.location.href = '/Event/Event';
    //});

});

function GetEventList() {
    ajaxCall("Get", false, '/Event/Event', null, function (result) {
        $("#divEventList").html(result.responseText);
        ApplyDatatable('tblEvent');

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
function showCalenderPopup(name) {

    ajaxCall("GET", false, '/Dashboard/CalenderPopupData?name=' + name, null, function (result) {
        if (result.status == true) {
            $("#EventTitle").text(result.data.title);
            $("#EventDescription").text(result.data.description);
            if (result.data.allDay) {
                $("#EventStartTime").text(result.data.start.split('T')[0] + ' ' + '(FullDayEvent)' );
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


