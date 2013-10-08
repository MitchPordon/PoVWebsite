using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PoVWebsite.Models;
using PoVWebsite.Content.Utility;

namespace PoVWebsite.Controllers.API
{
    public class UserController : ApiController
    {
        PoVEntities db = new PoVEntities();

        // GET api/user/5
        public User Get(int id)
        {
            User user = db.Users.SingleOrDefault(m => m.id == id);
            return user;
        }

        public User Post(string username, string password, int id)
        {
            if (General.ValidUserCredentials(username, password, id))
            {
                User user = db.Users.SingleOrDefault(m => m.id == id);
                return user;
            }
            HttpResponseException exep = new HttpResponseException(HttpStatusCode.BadRequest);
            throw exep;
        }

        public IEnumerable<Available> PostAvailability(string username, string password, int dayRange = 7)
        {
            User user = db.Users.SingleOrDefault(m => (m.username.Equals(username) && m.password.Equals(password)));
            return user.Availables.Where(m => m.start.DayOfYear >= DateTime.Now.DayOfYear - dayRange);
        }
        // POST api/user
        public void Post([FromBody]string value)
        {
        }

        public HttpResponseMessage PostSubmitPushURI(string username, string password, string phone, int userID, string url)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            if (General.ValidUserCredentials(username, password, userID))
            {
                try
                {
                    PushContact existingPush = db.PushContacts.SingleOrDefault(m => (m.user_id == userID && m.phone.Equals(phone, StringComparison.CurrentCultureIgnoreCase)));
                    if (existingPush != null)
                    {
                        if (existingPush.phone_uri.Equals(url, StringComparison.CurrentCultureIgnoreCase))
                        {
                            resp.StatusCode = HttpStatusCode.NoContent;
                            return resp;
                        }
                        existingPush.phone_uri = url;
                        db.SaveChanges();
                        resp.StatusCode = HttpStatusCode.OK;
                        resp.Content = new StringContent("Push data updated");
                        return resp;
                    }
                    db.PushContacts.Add(new PushContact { user_id = userID, phone = phone, phone_uri = url });
                    db.SaveChanges();
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = new StringContent("Push data added");
                    return resp;
                }
                catch(Exception e)
                {
                    resp.StatusCode = HttpStatusCode.Conflict;
                    resp.Content = new StringContent("Could not save or update database.");
                    return resp;
                }
            }
            resp.StatusCode = HttpStatusCode.Forbidden;
            resp.Content = new StringContent("Invalid username or password");
            return resp;
        }
    }
}
