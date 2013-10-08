using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using PoVWebsite.Models;
using System.Net;

namespace PoVWebsite.Filters
{
    public class TokenFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext filterContext)
        {
            PoVEntities db = new PoVEntities();
            
            if(filterContext.Request.Headers.Contains("authToken") && filterContext.Request.Headers.GetValues("authToken") != null)
            {
                string authToken = Convert.ToString(filterContext.Request.Headers.GetValues("authToken").FirstOrDefault());
                if (filterContext.Request.Headers.Contains("id") && filterContext.Request.Headers.GetValues("id") != null)
                {
                    int userID;
                    if (int.TryParse(filterContext.Request.Headers.GetValues("id").FirstOrDefault(), out userID))
                    {
                        if (filterContext.Request.Headers.Contains("appKey") && filterContext.Request.Headers.GetValues("appKey") != null)
                        {
                            string appKey = filterContext.Request.Headers.GetValues("appKey").FirstOrDefault();
                            Token token = db.Tokens.SingleOrDefault(m => (m.public_key.Equals(appKey) && m.user_id == userID));
                            string hashedToken = Content.Utility.RSAClient.Hash(token.auth_token);
                            return;
                        }
                        filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                        filterContext.Response.ReasonPhrase = "Missing your app key";
                        return;
                    }
                    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                    filterContext.Response.ReasonPhrase = "User info invalid";
                    return;
                }
                filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                filterContext.Response.ReasonPhrase = "Missing the user";
                return;
            }
            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            filterContext.Response.ReasonPhrase= "Missing a token";
            return;
            //base.OnAuthorization(filterContext);
        }
    }
}