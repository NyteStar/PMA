using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMA.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            HttpCookie uCookie = Request.Cookies["sessionCookie"];

            if (uCookie != null)
            {
                return View();
            }

            else
            {
                return RedirectToAction("login", "Home");
            }
        }
    }
}