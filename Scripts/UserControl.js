$(document).ready(function () {
    $(".button").click(function () {
        var clickID = "#" + $(this).attr("id") + "_content";
        var newDiv = $(clickID);
        if (newDiv.hasClass("active")) {

        } else {

            var oldDiv = $(".active");
            /*Close open tab */

            oldDiv.slideUp();
            oldDiv.fadeOut(5000);
            oldDiv.css("display", "none");
            oldDiv.removeClass("active");
            /*open new active tab*/
            newDiv.addClass("active");

            newDiv.fadeIn(1000);
            newDiv.css("display", "block");
        }
    }
    )
}
);

function removeTab(clickID){

}