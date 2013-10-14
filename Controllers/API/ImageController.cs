using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PoVWebsite.Models;
using PoVWebsite.Content.Utility;

namespace PoVWebsite.Controllers.API
{
    public class ImageController : ApiController
    {
        PoVEntities db = new PoVEntities();
        // POST api/Image?
        public bool PostImage(string username, string password, int userID)
        {
            HttpContent requestContent = Request.Content;
            Task<byte[]> t = requestContent.ReadAsByteArrayAsync();
            t.Wait();
            byte[] imageData = t.Result;
                if (General.ValidUserCredentials(username, password, userID))
                {
                    return PoVWebsite.Content.Utility.Image.CreateImage(imageData, userID);
                }
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                response.Content = new StringContent("Incorrect Username or Password");
                throw new HttpResponseException(response);
            

        }
        //Get api/Image?
        public HttpResponseMessage GetImage()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, true);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", 1));
            return response;
        }

    }
}
