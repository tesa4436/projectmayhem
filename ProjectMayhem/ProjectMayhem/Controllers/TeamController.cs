using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProjectMayhem.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectMayhem.Controllers
{
    public class TeamController : Controller
    {

        private ApplicationUserManager _userManager;

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

        TeamManager TM = new TeamManager();

        // GET: Team
        [Authorize]
        public ActionResult Members(string Id)
        {
            
            string LeadId;
            if (String.IsNullOrEmpty(Id))
                LeadId = User.Identity.GetUserId();
            else
            {
                var ReqUser = UserManager.FindById(Id);
                if (TM.CheckIfLead(ReqUser, User.Identity.GetUserId()))
                    LeadId = Id;
                else return View("Error");
            }

            Models.MembersViewModel viewModel = new Models.MembersViewModel();
            viewModel.Employees = TM.GetMembersById(LeadId);
            return View(viewModel);
        }

    }
}