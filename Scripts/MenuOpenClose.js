$(document).ready(function () {
    $("#side_button").click(function () {
        var openClose = $(this).attr("class");

        if (openClose == "open") {
            CloseMenu();
        } else {
            OpenMenu();
        }

    }
    )
}
);

function OpenMenu() {
    $("#side_button_wrapper").css("right", "92%");
    $("#side_contents").css("visibility", "visible");
    $("#side_button").attr("class", "open");
    $("#side_button").html("C<br />L<br />O<br />S<br />E");
    $.cookie('menuStatus', 'open');
}

function CloseMenu() {
    $("#side_button_wrapper").css("right", "1%");
    $("#side_contents").css("visibility", "hidden");
    $("#side_button").attr("class", "closed");
    $("#side_button").html("O<br />P<br />E<br />N");
    $.cookie('menuStatus', 'closed');
}

