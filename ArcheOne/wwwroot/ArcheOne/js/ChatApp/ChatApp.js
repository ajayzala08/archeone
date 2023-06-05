var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

$(document).ready(function () {
    GetUserList();
});

connection.on("ReceiveMessage", function (user, message) {
    var encodedUser = $("<div />").text(user).html();
    var encodedMsg = $("<div />").text(message).html();
    $("#chatBox").append("<p><strong>" + encodedUser + "</strong> : " + encodedMsg + "</p>");
});

$("#sendButton").click(function () {
    var user = $("#ddlUsers :selected").text();
    var message = $("#message").val();
    connection.invoke("SendMessage", user, message);
    $("#message").val("").focus();
});

connection.start().then(function () {
    console.log("Connected!");
}).catch(function (err) {
    console.error(err.toString());
});

function GetUserList() {
    ajaxCall("Get", false, '/Chat/GetUserList', null, function (result) {
        console.log(result);
        debugger
        if (result.status == true) {
            $("#ddlUsers").html('');
            //$("#ddlUsers").append('<option value="0">--- Select ---</option>');
            $.each(result.data, function (index, value) {
                $("#ddlUsers").append('<option  value="' + value.id + '">' + value.fullName + '</option>');
            });
        }
        else {
            Toast.fire({ icon: 'error', title: result.message });
        }
    });
}