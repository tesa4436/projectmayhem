using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProjectMayhem.Models;
using ProjectMayhem.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        public ActionResult Members(string EmpId)
        {
            string Id = EmpId;
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

            MembersViewModel viewModel = new Models.MembersViewModel();
            viewModel.Employees = TM.GetMembersById(LeadId);
            
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Members(MembersViewModel model)
        {
            string EmpId = Request.QueryString["EmpId"];
            var ReqUser = UserManager.FindById(model.EmpId);
            if (TM.CheckIfLead(ReqUser, User.Identity.GetUserId()))
            {
                var newLead = TM.GetEmployeeIdByUsername(model.NewLeadUserName);
                if (model.NewLeadUserName == ReqUser.UserName)
                    ModelState.AddModelError("", "Cant assign to himself");
                else if (newLead != null) {
                    ReqUser.teamLead = UserManager.FindById(newLead);
                    UserManager.Update(ReqUser);
                }
                else ModelState.AddModelError("", "No such user exist");
                if (!string.IsNullOrEmpty(EmpId))
                    model.Employees = TM.GetMembersById(EmpId);
                else
                    model.Employees = TM.GetMembersById(User.Identity.GetUserId());
                return View(model);
                //return RedirectToAction("Members", "Team", new { EmpId = EmpId });
            }
            return View("Error"); //If you get here - you shouldn't be here
        }

    }
}