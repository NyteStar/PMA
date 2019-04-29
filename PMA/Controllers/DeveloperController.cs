using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using PMA.Models;
using Image = System.Drawing.Image;

namespace PMA.Controllers
{
    public class DeveloperController : Controller
    {
        PMAEntities db = new PMAEntities();
        dynamic model = new ExpandoObject();

        public ActionResult SaveImage()
        {
            HttpCookie uCookie = Request.Cookies["sessionCookie"];
            if (uCookie != null)
            {
                int uId = Int32.Parse(uCookie.Value);
                User developer = db.Users.Single(dev => dev.id == uId);
                Image temp = Image.FromFile("C:\\Users\\NyteStar\\Downloads\\profile2.jpg");
                MemoryStream strm = new MemoryStream();
                temp.Save(strm, ImageFormat.Png);
                byte[] imageByteArray = strm.ToArray();
                developer.image = imageByteArray;
                db.SaveChanges();
                return RedirectToAction("Details", "Developer");
            }
            return RedirectToAction("login", "Home");
        }

        public ActionResult Details()
        {
            HttpCookie uCookie = Request.Cookies["sessionCookie"];
            
            if (uCookie!=null)
            {   int uId = Int32.Parse(uCookie.Value);
                model.Developer = db.Users.Single(dev => dev.id == uId);
                if (model.Developer.firstName != null)
                {
                    model.Skills = FindSkills(uId);
                    FindProjects(uId);

                    return View(model);
                }
            }
            return RedirectToAction("login", "Home");  
        }

        public EmptyResult FindProjects(int id)
        {
            var newProjects = db.Projects.Where(x => x.ProjectToUsers.Any(r => id.Equals(r.u_ID) && r.response==null));

            foreach (Project proj in newProjects)
            {
                proj.Cust = FindUser(proj.pCust_ID);
                proj.Pm = FindUser(proj.pPM_ID);
            }
            model.newProjects = newProjects;

            var acceptedProjects = db.Projects.Where(x => x.ProjectToUsers.Any(r => id.Equals(r.u_ID) && r.response ==1));

            foreach (Project proj2 in acceptedProjects)
            {
                proj2.Cust = FindUser(proj2.pCust_ID);
                proj2.Pm = FindUser(proj2.pPM_ID);
            }
            model.acceptedProjects = acceptedProjects;
            return new EmptyResult();
        }

        public string FindUser(int id)
        {
            User user = new User();
            user = db.Users.Single(usr => usr.id == id);
            string name = user.firstName + " " + user.lastName;
            return (name);
        }

        public IQueryable<Skill> FindSkills(int id)
        {
            var skills = db.Skills
                .Where(x => x.Users.Any(r => id.Equals(r.id)));
            return skills;
        }

        public ActionResult ProjectRespond(int uId,int pId, int res)
        {
            var project = db.ProjectToUsers.Single(r => r.u_ID == uId && r.p_ID == pId);
            if (project != null)
            {
                project.response = res;
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Developer");
        }

        public ActionResult ProjectDrop(int uId, int pId)
        {
            var project = db.ProjectToUsers.Single(r => r.u_ID == uId && r.p_ID == pId);
            if (project != null)
            {
                db.ProjectToUsers.Remove(project);
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Developer");
        }
    }
}