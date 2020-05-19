using ProjectMayhem.DbEntities;
using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ProjectMayhem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AddTopics();
        }

        private void AddTopics()
        {
            using(var contxt = ApplicationDbContext.Create())
            {
                var topics = new Topic();
                topics.Title = "Graphical design";
                topics.Description = "Very good thing";
                contxt.topics.Add(topics);
                contxt.SaveChanges();
            }
        }

    }
}
