using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PoVWebsite.Models;

namespace PoVWebsite.Controllers
{
    public class HomeController : Controller
    {
        PoVEntities db = new PoVEntities();

        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return View(db.Users.SingleOrDefault(m=> m.username.Equals(User.Identity.Name)));
            }
            return View();
        }

        //Don't beleive this needs to be here
        public ActionResult test()
        {
            return PartialView("_SideMenu.cshtml");
        }

        public ActionResult GetCollage()
        {
            return PartialView("_Collage", db.Pictures.ToList());
        }

    }
}
