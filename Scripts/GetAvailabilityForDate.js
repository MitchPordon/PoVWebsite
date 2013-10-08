function GetAvailabilityForDate() {

        $("#Current_Availability").load("/User/GetAvailabilityForDate", { dateString: $('#datepicker').val() });

}
