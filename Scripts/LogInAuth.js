$(document).ready(function () {
    var $loginButton = $('button.login');

    $loginButton.live('click', function () {
        $('span.Login_Error').html("");
        var theUsername = $('#username').val();
        var thePassword = $('#password').val();
       // $.ajax({
            //type: "POST",
            //url: "/Account/LogIn", // the method we are calling
            //data: { username: theUsername, password: thePassword },
            //dataType: "json",
           // success: function (result) {
               // window.location = window.location.pathname;
                //$('#side_contents').load("/test");
           // },
            //error: function (result) {
                //$('span.Login_Error').html("Username and/or password are incorrect.");
                //Flash($('span.Login_Error'));
           // }
        //});
        $.post("/Account/LogIn",
            {
                username: theUsername,
                password: thePassword
            },
            function (data, status, xhr) {
                if (data.Success) {
                    window.location = window.location.pathname;
                }
                else {
                    $('span.Login_Error').html("Username and/or password are incorrect.");
                    Flash($('span.Login_Error'));
                }

            }
        );

    }
    );



}
);

function Flash(e) {
    e.css("background-color", "red").fadeTo("fast", .5).fadeTo("fast", 1);

}