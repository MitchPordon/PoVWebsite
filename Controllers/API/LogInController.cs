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
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace PoVWebsite.Controllers.API
{
    public class LogInController : ApiController
    {

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

        public async Task<HttpResponseMessage> PostLogIn2(string username)
        {
            PoVEntities db = new PoVEntities();
            string appKey, blob, data;
            if (!Request.Headers.Contains("appKey") && Request.Headers.GetValues("appKey")!= null)
            {
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                resp.ReasonPhrase = "Missing the app key";
                return resp;
            }
            appKey = Convert.ToString(Request.Headers.GetValues("appKey").FirstOrDefault());
            using (StreamReader sr = new StreamReader(await Request.Content.ReadAsStreamAsync()))
            {
                blob = await sr.ReadLineAsync();
                data = await sr.ReadLineAsync();
            }
            if(blob == null || data==null)
            {
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                resp.ReasonPhrase = "Missing required data";
                return resp;
            }
            string param = PoVWebsite.Content.Utility.RSAClient.Decrypt(data);
            if(!param.Contains('=')){
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                resp.ReasonPhrase = "Data in invalid format";
                return resp;
            }
            string password = param.Split(new char[] { '=' })[1];
            string hashedData = PoVWebsite.Content.Utility.RSAClient.Hash(param + "&username=" + username);
            if (hashedData.Equals(blob))
            {
                User match = db.Users.SingleOrDefault(m => (m.username.Equals(username) && m.password.Equals(password)));
                if (match == null)
                {
                    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    resp.ReasonPhrase = "Username/Password incorrect";
                    return resp;
                }
                else
                {
                    string token = PoVWebsite.Content.Utility.RSAClient.NewToken();
                    Token existingToken = match.Tokens.SingleOrDefault(m => m.public_key.Equals(appKey));
                    if (existingToken != null)
                    {
                        existingToken.auth_token = token;
                        existingToken.expires = DateTime.Now;
                        db.Entry(existingToken).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    else
                    {
                        Token newToken = new Token { user_id = match.id, public_key = appKey, User = match, auth_token = token };
                        newToken.expires = DateTime.UtcNow;
                        db.Tokens.Add(newToken);
                        db.SaveChanges();
                    }
                    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.ReasonPhrase = "Login Successful";
                    resp.Headers.Add("authToken", token);
                    resp.Content = new StringContent("userID=" + match.id);
                    return resp;
                }
            }
            else
            {
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                resp.ReasonPhrase = "Incorrect information";
                return resp;
            }

                
        }

    }


}
