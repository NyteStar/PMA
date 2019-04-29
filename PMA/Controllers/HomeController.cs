using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMA.Models;

namespace PMA.Controllers
{
    public class HomeController : Controller
    {
        PMAEntities db = new PMAEntities();
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult login(FormCollection form)
        {

            string username = form["username"].ToString();
            string password = form["password"].ToString();
            var usr = (from u in db.Users
                where u.username == username && u.password == password
                select u).FirstOrDefault();
            if (usr != null)
            {
                HttpCookie sessionCookie = new HttpCookie("sessionCookie");
                sessionCookie.Value = usr.id.ToString();
                Response.Cookies.Add(sessionCookie);

                if (usr.type == 1) //Admin
                {
                    return RedirectToAction("Index", "Dashboard");
                }

                else if (usr.type == 3) //Developer
                {
                    return RedirectToAction("Details", "Developer");
                }
            }
            TempData["message"] = "Wrong Username or Password!";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logout()
        {
            var expiredCookie = new HttpCookie("sessionCookie")
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            Response.Cookies.Add(expiredCookie);
            Request.Cookies.Clear();
            return View("login");
        }
    }
}