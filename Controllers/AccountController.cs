using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PoVWebsite.Models;
using System.IO;

namespace PoVWebsite.Controllers
{
    public class AccountController : Controller
    {
        PoVEntities db = new PoVEntities();
        //
        // GET: /Account/

        
        [HttpPost]
        public ActionResult LogIn(string username, string password)
        {
            
                User match = db.Users.SingleOrDefault(m => m.username.Equals(username));
                if (match != null)
                {
                    if (match.password.Equals(password)) //Success
                    {                        
                        FormsAuthentication.SetAuthCookie(match.username, false);
                        return Json(new { Success = true });
                    }
                }
                return Json(new { Success = false });
        }

        [HttpPost]
        public JsonResult LogInTest(string username, string password)
        {
            if (ModelState.IsValid)
            {
                User match = db.Users.SingleOrDefault(m => m.username.Equals(username));
                if (match != null)
                {
                    if (match.password.Equals(password)) //Success
                    {
                        FormsAuthentication.SetAuthCookie(username, false);
                        return Json("success");
                    }
                    ModelState.AddModelError("pass", "The password you entered is incorrect.");
                    return Json("error");
                }
                ModelState.AddModelError("username", "The username you entered does not exist.");
                return Json("error");
            }
            return Json("error");
        }
        
        public ActionResult LogIn()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            using (db)
            {
                if (ModelState.IsValid)
                {
                    List<User> users = db.Users.Where(m=> (m.username.Equals(user.username) ||  m.email.Equals(user.email))).ToList();
                    if (users.Count == 0)//No one has this username or email
                    {
                        user.account = "basic";
                        user.role = "user";
                        user.email_message = false;
                        user.text_message = false;
                        user.app_message = false;
                        user.changed = true;
                        db.Users.Add(user);
                        db.SaveChanges();
                        FormsAuthentication.SetAuthCookie(user.username, false);
                        Directory.CreateDirectory("C:\\inetpub\\wwwroot\\pov.jschroed.com\\UserImages\\" + user.username);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        if (users.SingleOrDefault(m => m.username.Equals(user.username)) != null) 
                            ModelState.AddModelError("Username", "Username is already taken.");
                        if (users.SingleOrDefault(m => m.email.Equals(user.email)) != null)
                            ModelState.AddModelError("email", "That e-mail address already has an account associated with it.");
                        return View();
                    }
                }
            }
            return View(user);

        }

        public ActionResult LogOut()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View(db.Users.SingleOrDefault(m => m.username.Equals(User.Identity.Name)));
        }

        public ActionResult Test()
        {
            return View();
        }
    }
}
