$(document).ready(function () {
    alert("Hi");
    LoadBirthdayWorkAniversaryHoliday();
});

function LoadBirthdayWorkAniversaryHoliday() {
    ajaxCallWithoutDataType("GET", false, '/Dashboard/GetBirthdayWorkAniversaryHoliday', null, function (result) {
        if (result.status == true) {
            let holidayCnt = result.data.holidays.length;
            $('#holidaycnt').text(holidayCnt + ' Holidays');
            $.each(result.data.holidays, function (data, value) {
                let strToAdd = '<tr><td>' + value.holidayName + '</td><td>' + value.holidayDate +'</td></tr>';
                $('#tblHoliday').append(strToAdd);
            })

            let birthdayCnt = result.data.birthdays.length;
            $('#birthdayCnt').text(birthdayCnt + ' Birthday');
            $.each(result.data.birthdays, function (data, value) {
                let strToAdd = '<li><img src=# alt="User Image"></img><a class=users-list-name href=#>' + value.employeeName + ' </a> <span class=users-list-date>' + value.birthdate +'</span></li>';
                $('#libirthday').append(strToAdd);
            })

            let anniversaryCnt = result.data.workAnniversaries.length;
            $('#birthdayCnt').text(anniversaryCnt + ' Anniversary');
            $.each(result.data.workAnniversaries, function (data, value) {
                let strToAdd = '<li><img src=' +"\\"+ value.employeeImagePath + ' alt="User Image"></img><a class=users-list-name href=#>' + value.employeeName + ' </a> <span class=users-list-date>' + value.joinDate + '</span></li>';
                $('#liAnniversary').append(strToAdd);
            })
        }
    });
}

