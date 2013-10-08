using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PoVWebsite.Models;

namespace PoVWebsite.Content.Utility
{
    public static class Availability
    {
        public static HtmlString TodaysAvailability(User user)
        {
            DateTime current = DateTime.Now;
            List<Available> availabilities = user.Availables.Where(m => (m.start.Date.Equals(current.Date)) && m.start.TimeOfDay.CompareTo(DateTime.Now.TimeOfDay) > -1).ToList();
            availabilities = availabilities.OrderBy(m=>m.start.TimeOfDay).ToList();
            string result = "Today's Availability<br/>";

            if (availabilities.Count != 0)
            {
                foreach (Available a in availabilities)
                {
                    result += a.start.TimeOfDay.ToString(@"hh\:mm") + "  -  " + a.end.TimeOfDay.ToString(@"hh\:mm") + "<br/>";
                }
            }
            else
            {
                result += "No availabilities set for today";
            }


            return new HtmlString(result) ;
        }
    }
}