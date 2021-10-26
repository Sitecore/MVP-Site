$(document).ready(function () {

    $("#btnWelcome").click(function (e) {
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: "/Application/Welcome",
            data: {
                
            },
            success: function (result) {
                alert('ok');
            },
            error: function (result) {
                alert('error');
            }
        });
    });


});