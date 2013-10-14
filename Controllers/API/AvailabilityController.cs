using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using PoVWebsite.Models;
using PoVWebsite.Filters;

namespace PoVWebsite.Controllers.API
{
    public class AvailabilityController : ApiController
    {
        
        PoVEntities db = new PoVEntities();
        //Verify for removal
        public string GetAvailabilities(string username, string password)
        {
           
            string response = "";
            User user = db.Users.SingleOrDefault(m=> m.username.Equals(User.Identity.Name));
            if(user != null){
                foreach(Available a in user.Availables)
                    response += a.start + "\n";
            }
            return response;
            HttpResponseMessage response2 = Request.CreateResponse(HttpStatusCode.Created, user);
            response2.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = user.id }));
            
            return response;

        }

        [TokenFilter]
        public HttpResponseMessage Get(string lastChecked)
        {
            DateTime LastChecked;
            if (!DateTime.TryParse(lastChecked, out LastChecked))
            {
                var resp = Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                resp.ReasonPhrase = "Invalid Date Format";
                return resp;
            }
            int id = int.Parse(Request.Headers.GetValues("id").FirstOrDefault());
            User user = db.Users.SingleOrDefault(m => m.id == id);
            List<Available> newAvailability = user.Availables.Where(m=>m.added > LastChecked).ToList();
            if (newAvailability == null)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            else if (newAvailability.Count < user.Availables.Count)
            {
                return Request.CreateResponse(HttpStatusCode.PartialContent, newAvailability);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, newAvailability);
            }
        }

        [TokenFilter]
        public string PostAvailabilities(string username, string password)
        {
            int userID = int.Parse(Request.Headers.GetValues("id").FirstOrDefault());
            string response = "";
            User user = db.Users.SingleOrDefault(m => m.id==userID);
            if (user != null)
            {
                foreach (Available a in user.Availables)
                    response += a.start + "\n";
            }
            return response;

        }
    }
}
