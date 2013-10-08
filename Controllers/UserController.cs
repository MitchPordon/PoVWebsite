using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PoVWebsite.Models;
using System.Web.Helpers;

namespace PoVWebsite.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        PoVEntities db = new PoVEntities();

        public ActionResult Index()
        {
            if(Request.IsAuthenticated)
                return View(db.Users.SingleOrDefault(m=> m.username.Equals(User.Identity.Name)));
            return RedirectToAction("Index", "Home");
                
        }

        [HttpPost]
        public ActionResult Index(User user, String sq)
        {
            User user2 = db.Users.SingleOrDefault(m => m.username.Equals(User.Identity.Name));

            user.username = sq;
            return View("updatePassword", user);
        }

        public ActionResult Available()
        {
            if (Request.IsAuthenticated)
                return View(db.Users.SingleOrDefault(m => m.username.Equals(User.Identity.Name)));
            return RedirectToAction("Index", "Home");
        }

        

        [HttpPost]
        public JsonResult Available(string start, string end)
        {
            User user = db.Users.SingleOrDefault(m => m.username.Equals(User.Identity.Name));
            Available newAvailable = new Available { user_id = user.id, User = user };
                try
                {
                    newAvailable.start = DateTime.Parse(start);
                }
                catch (FormatException e)
                {
                    return Json(new { Success = false });
                }
                try
                {
                    newAvailable.end = DateTime.Parse(end);
                }
                catch (FormatException e)
                {
                    return Json(new { Success = false });
                }
                if (newAvailable.end.TimeOfDay.CompareTo(newAvailable.start.TimeOfDay) == -1) //Compare to returns -1 if the first object is less than the second object(the end time is less than the start time)
                {
                    return Json(new { Success = false });
                }
                List<Available> timesForDate = user.Availables.Where(m => m.start.Date.Equals(newAvailable.start.Date)).ToList();
                newAvailable = checkForOverlap(newAvailable, timesForDate);
                if (newAvailable != null)
                    db.Availables.Add(newAvailable);
                db.SaveChanges();
                int tempID = newAvailable.id;
                return Json(new { Success = true, newID=newAvailable.id });


            
            
        }
        [HttpPost]
        public JsonResult UpdateAvailable(string id, string start,string end){
            User user = db.Users.SingleOrDefault(m => m.username.Equals(User.Identity.Name));
            int Id = int.Parse(id);
            Available newAvailable = new Available { user_id = user.id, User = user, id=Id};
            Available oldAvailable = db.Availables.SingleOrDefault(m => m.id == Id);
            //db.Availables.Remove(oldAvailable);
            try
            {
                newAvailable.start = DateTime.Parse(start);
            }
            catch (FormatException e)
            {
                return Json(new { Success = false });
            }
            try
            {
                newAvailable.end = DateTime.Parse(end);
            }
            catch (FormatException e)
            {
                return Json(new { Success = false });
            }
            if (newAvailable.end.TimeOfDay.CompareTo(newAvailable.start.TimeOfDay) == -1) //Compare to returns -1 if the first object is less than the second object(the end time is less than the start time)
            {
                return Json(new { Success = false });
            }
            List<Available> timesForDate = user.Availables.Where(m => m.start.Date.Equals(newAvailable.start.Date)).ToList();
            newAvailable = checkForOverlap(newAvailable, timesForDate);
            if (newAvailable != null)
            {
                oldAvailable.start = newAvailable.start;
                oldAvailable.end = newAvailable.end;
            }
                //db.Availables.Add(newAvailable);
            db.SaveChanges();
            return Json(new { Success = true });
        }

        [HttpPost]
        public JsonResult RemoveAvailable(string id)
        {
            User user = db.Users.SingleOrDefault(m => m.username.Equals(User.Identity.Name));
            if(user==null)
                return Json(new {Success = false});
            int Id = int.Parse(id);
            Available remove = user.Availables.SingleOrDefault(m => m.id == Id);
            db.Availables.Remove(remove);
            db.SaveChanges();
            return Json(new { Success = true });
        }

        private Available checkForOverlap(Available newAvailable, List<Available> availabilities)
        {
            //Possible problem is an availability is added that fits inside an exist availability(start or end time would be adjusted but not both)
            TimeSpan start = newAvailable.start.TimeOfDay;
            TimeSpan end = newAvailable.end.TimeOfDay;
            foreach(Available a in availabilities)
            {
                if(a.id==newAvailable.id)
                {
                    availabilities.Remove(a);
                    return checkForOverlap(new Available { start = a.start, end = newAvailable.end, User = newAvailable.User, user_id = newAvailable.user_id}, availabilities);
                }
                //This first check is working fine
                //Merges availabilities if the start time is within an existing availability
                if (start.CompareTo(a.end.TimeOfDay) < 1) //Check if the new availabilities start time is before the end time
                {
                    if (start.CompareTo(a.start.TimeOfDay) > -1) //Check if the new start time is after the start time(new start time is between start and end time)
                    {
                        newAvailable.start.Subtract(start);
                        newAvailable.start.Add(a.start.TimeOfDay);
                        db.Availables.Remove(a);
                        availabilities.Remove(a);
                        return checkForOverlap(new Available { start = a.start, end = newAvailable.end, User = newAvailable.User, user_id = newAvailable.user_id}, availabilities);
                        
                    }
                }
                // This check does not seem to be working
                //Merges availabilities if the end time is within an existing availability
                if(end.CompareTo(a.start.TimeOfDay) > -1) //Check if the end of the new availability is after the start time
                {
                    if (end.CompareTo(a.end.TimeOfDay) < 1)  //Check if the new end time is before the end time(new end time is between start and end time)
                    {
                        ViewBag.Test = "Inside second check";
                        newAvailable.end.Subtract(end);
                        newAvailable.end.Subtract(a.end.TimeOfDay);
                        db.Availables.Remove(a);
                        availabilities.Remove(a);
                        return checkForOverlap(new Available { start = newAvailable.start, end = a.end, User = newAvailable.User, user_id = newAvailable.user_id }, availabilities);
                        
                    }
                       
                }
                //Checks if an existing availability exists within the new availability. If one does it is removed
                if (start.CompareTo(a.start.TimeOfDay) != 1) //If start is before existing start
                {
                    if (end.CompareTo(a.end.TimeOfDay) != -1)//If end is after existing end
                    { 
                        availabilities.Remove(a);
                        db.Availables.Remove(a);
                        return checkForOverlap(newAvailable, availabilities);
                    }

                }
                //Checks if the new availability would exist inside an existing one, If so the new one is not added
                else if (start.CompareTo(a.start.TimeOfDay) != -1)  //If start is after the existing start time
                {
                    if (end.CompareTo(a.end.TimeOfDay) != 1) //If end is before the existing end time
                    {
                        return null; //The main method will check if null
                    }
                }

            }
            return newAvailable;


        }

        [HttpPost]
        public HtmlString GetAvailabilityForDate(string dateString)
        {
            try
            {
                User user = db.Users.SingleOrDefault(m => m.username.Equals(User.Identity.Name));
                DateTime date = DateTime.Parse(dateString);
                List<Available> availabilities = user.Availables.Where(m => m.start.Date.Equals(date.Date)).OrderBy(m => m.start.Hour).ToList();
                string result = "Availability for " + date.Date.ToString("d") + ":<br/>";
                if (availabilities.Count != 0)
                {
                    foreach (Available a in availabilities)
                    {
                        result += "Start Time: " + a.start.TimeOfDay.ToString(@"hh\:mm") + "      End Time: " + a.end.TimeOfDay.ToString(@"hh\:mm") + "<br/>";
                    }
                }
                else
                {
                    result += "No availability has been set for this date.";
                }

                return new HtmlString(result);
            }
            catch
            {

            }
            return new HtmlString("Error");

        }          
        

        [HttpPost]
        public ActionResult getMonthAvailabilty(string date)
        {
            string DateFormat = "yyyy-MM-ddTHH:mm:ssZ";
            User user = db.Users.SingleOrDefault(m => m.username.Equals(User.Identity.Name));

            DateTime startDate = DateTime.Parse(date).Date;
            DateTime startDay = startDate.AddDays(-(startDate.Day-1));
            DateTime endDay = startDate.AddMonths(1).AddDays(-startDate.Day).Add(new TimeSpan(23,59,59));
            List<Available> availabilities = user.Availables.Where(m => (m.start.Date.CompareTo(startDay.Date) > -1 && m.end.Date.CompareTo(endDay.Date) < 1)).ToList();
            List<Object> os = new List<object>();
            foreach (Available a in availabilities)
            {
                os.Add(new { title = "", start = a.start.ToString(DateFormat), end = a.end.ToString(DateFormat), allDay = false, id = a.id });
            }
            return Json(os, JsonRequestBehavior.AllowGet);
        }

        public ActionResult updatePassword(User user){
            return View(user);
        }

        #region Old Methods
        //Old Availability
        /*
        [HttpPost]
        public ActionResult Available(string start_time, string end_time, string date)
        {
            User user = db.Users.SingleOrDefault(m => m.username.Equals(User.Identity.Name));
            Available newAvailable = new Available { user_id = user.id, User = user };
            DateTime baseDate = DateTime.Parse(date);
            if (ModelState.IsValid)
            {
                try
                {
                    newAvailable.start = baseDate.Add(DateTime.Parse(start_time).TimeOfDay);
                }
                catch (FormatException e)
                {
                    ModelState.AddModelError("start", "Incorrect time format(HH:MM).");
                    return View(user);
                }
                try
                {
                    newAvailable.end = baseDate.Add(DateTime.Parse(end_time).TimeOfDay);
                }
                catch (FormatException e)
                {
                    ModelState.AddModelError("end", "Incorrect time format(HH:MM).");
                    return View(user);
                }
                if (newAvailable.end.TimeOfDay.CompareTo(newAvailable.start.TimeOfDay) == -1) //Compare to returns -1 if the first object is less than the second object(the end time is less than the start time)
                {
                    ModelState.AddModelError("end", "End time must be after the start time.");
                    return View(user);
                }
                List<Available> timesForDate = user.Availables.Where(m => m.start.Date.Equals(newAvailable.start.Date)).ToList();
                newAvailable = checkForOverlap(newAvailable, timesForDate);
                if (newAvailable != null)
                    db.Availables.Add(newAvailable);
                db.SaveChanges();


            }
            return View(user);
        }
         * */
        #endregion

    }
}
