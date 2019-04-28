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
using PMA.Models;
using Image = System.Drawing.Image;

namespace PMA.Controllers
{
    public class DeveloperController : Controller
    {
        public ActionResult Index(int id)
        {
            PMAEntities db = new PMAEntities();
            User developer = db.Users.Single(dev => dev.id == id);
            Image temp = Image.FromFile("C:\\Users\\NyteStar\\Downloads\\profile.png");
            MemoryStream strm = new MemoryStream();
            temp.Save(strm, ImageFormat.Png);
            byte[] imageByteArray = strm.ToArray();
            developer.image = imageByteArray;
            db.SaveChanges();
            return View(developer);
        }

        public ActionResult Details(int id)
        {
            dynamic myModel = new ExpandoObject();
            PMAEntities db = new PMAEntities();
            myModel.Developer = db.Users.Single(dev => dev.id == id);
            if (myModel.Developer.firstName != null)
                myModel.Projects = FindProjects(id);
            myModel.Skills = FindSkills(id);
            return View(myModel);
        }

        public IQueryable<Project> FindProjects(int id)
        {
            PMAEntities db = new PMAEntities();
            var projects = db.Projects
                .Where(x => x.Users.Any(r => id.Equals(r.id)));
            return projects;
        }

        public IQueryable<Skill> FindSkills(int id)
        {
            PMAEntities db = new PMAEntities();
            var skills = db.Skills
                .Where(x => x.Users.Any(r => id.Equals(r.id)));
            return skills;
        }
    }
}