using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using PoVWebsite.Models;
using System.Web.Security;
using System.Drawing;
using System.Web;

namespace PoVWebsite.Controllers.API
{
    public class LogInController : ApiController
    {

        public bool GetLogIn(string username, string password)
        {
            PoVEntities db = new PoVEntities();
            User match = db.Users.SingleOrDefault(m => m.username.Equals(username));
            if (match != null)
            {
                if (match.password.Equals(password)) //Success
                {
                    FormsAuthentication.SetAuthCookie(match.username, false);
                    //return new HttpResponseMessage(HttpStatusCode.OK);
                    return true;
                }
            }
            //return new HttpResponseMessage(HttpStatusCode.BadRequest);
            return false;
        }

        public HttpResponseMessage PostLogIn(string username, string password)
        {
            PoVEntities db = new PoVEntities();
            
            HttpResponseMessage resp = new HttpResponseMessage();
            User match = db.Users.SingleOrDefault(m => m.username.Equals(username));
            if (match != null)
            {
                if (match.password.Equals(password)) //Success
                {
                    FormsAuthentication.SetAuthCookie(match.username, false);
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = new StringContent("userID=" + match.id);
                    resp.Headers.Add("Public_Key", PoVWebsite.Content.Utility.RSAClient._publicKey);
                    resp.Headers.Add("Test_Encrypt", PoVWebsite.Content.Utility.RSAClient.Encrypt("test"));
                    resp.Headers.Add("Test_Decrypt", PoVWebsite.Content.Utility.RSAClient.Decrypt("110,220,225,92,85,57,90,132,158,252,84,107,220,149,19,195,6,110,108,135,165,95,4,218,110,24,181,166,191,237,218,28,25,234,187,125,27,217,46,211,250,100,139,27,158,202,67,182,204,162,99,168,103,17,252,163,166,115,27,231,51,83,243,116,25,12,100,19,226,231,220,32,131,78,16,78,146,213,251,48,174,100,41,122,6,244,231,77,201,161,42,219,170,151,146,193,42,202,161,97,159,138,200,71,136,62,19,92,115,192,239,16,112,168,197,217,193,248,67,207,197,239,226,226,156,94,13,147"));
                    return resp;
                }
            }
            resp.StatusCode = HttpStatusCode.Unauthorized;
            resp.Content = new StringContent("Username or password were incorrect.");
            return resp;
        }

        public HttpResponseMessage GetTest()
        {
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                //HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                //HttpCookie cookie2 = HttpContext.Current.Request.Cookies[0];
                //resp.Content = new StringContent(cookie.Value);
                if (!User.Identity.IsAuthenticated)
                {
                    resp.StatusCode = HttpStatusCode.BadRequest;
                    resp.Content = new StringContent("didnt work");
                }
                else
                {

                    resp.Content = new StringContent(User.Identity.Name);
                }
                return resp;

            }
            catch
            {
                resp.StatusCode = HttpStatusCode.BadGateway;
                return resp;
            }

        }

        public HttpResponseMessage GetTest2()
        {
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

    }


}
