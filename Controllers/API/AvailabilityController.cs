using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using PoVWebsite.Models;

namespace PoVWebsite.Controllers.API
{
    public class AvailabilityController : ApiController
    {
        
        PoVEntities db = new PoVEntities();
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

        public string PostAvailabilities(string username, string password)
        {
            string response = "";
            User user = db.Users.SingleOrDefault(m => (m.username.Equals(username) && m.password.Equals(password)));
            if (user != null)
            {
                foreach (Available a in user.Availables)
                    response += a.start + "\n";
            }
            return response;

        }
    }
}
