using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using ProjectMayhem.DbEntities;
using ProjectMayhem.Models;
using ProjectMayhem.Services;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectMayhem.Controllers
{
    public class ReportController : Controller
    {
        private ApplicationUserManager _userManager;
        private TopicManager _topicManager = new TopicManager();
        private UserManager _userDataManager = new UserManager();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize]
        public ActionResult SelectReport()
        {
            var data = new ReportViewModel
            {
                Topics = _topicManager.getAllTopics()
            };
            return View("Report", data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetPeopleByTopic(ReportViewModel model)
        {
            var topicId = model.SelectedTopic.TopicsId;

            model.Users = _userDataManager.GetUserByLearnedTopic(topicId);

            return View("UserList");
        }
    }
}