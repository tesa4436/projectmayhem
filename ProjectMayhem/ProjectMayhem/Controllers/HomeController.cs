using Microsoft.AspNet.Identity;
using ProjectMayhem.DbEntities;
using ProjectMayhem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectMayhem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }
        
    }
}