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
        public ActionResult Members(string LeadId)
        {
            string Id = LeadId;
            string CurrentLeadId;

            if (String.IsNullOrEmpty(Id))
                CurrentLeadId = User.Identity.GetUserId();
            else
            {
                var ReqUser = UserManager.FindById(Id);
                if (TM.CheckIfLead(ReqUser, User.Identity.GetUserId()))
                    CurrentLeadId = Id;
                else return View("Error"); //If you get here you shouldnt be here
            }

            MembersViewModel viewModel = new MembersViewModel();
            viewModel.Employees = TM.GetMembersById(CurrentLeadId);
            viewModel.AllUsers = UserManager.Users.ToList();
            return View(viewModel);
        }
        

        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Members(MembersViewModel model)
        {
            string EmpId = Request.QueryString["LeadId"];
            model.AllUsers = UserManager.Users.ToList();
            if (!string.IsNullOrEmpty(EmpId))
                model.Employees = TM.GetMembersById(EmpId);
            else
                model.Employees = TM.GetMembersById(User.Identity.GetUserId());

            Debug.WriteLine("Selected employee is " + model.EmpId);
            if (string.IsNullOrEmpty(model.EmpId))
            {
                ModelState.AddModelError("", "Team Member not selected");
                return View(model);
            }
            if(string.IsNullOrEmpty(model.NewLeadId))
            {
                ModelState.AddModelError("", "Leader Not selected");
                return View(model);
            }

            var ReqUser = UserManager.FindById(model.EmpId);

            if (TM.CheckIfLead(ReqUser, User.Identity.GetUserId()))  //Checking if you are a higher leader
            {
                var newLead = model.NewLeadId;    // Got an employee Id by his username

                if (model.NewLeadId == ReqUser.Id)                      // Checking if not assigning the person to himself
                    ModelState.AddModelError("", "Cant assign to himself");
                else if (newLead != null) {                                        // Checking if newLead is legit
                    var NewLeaderObj = UserManager.FindById(newLead);
                    if (TM.CheckIfLead(ReqUser, NewLeaderObj.Id) || !TM.CheckIfLead(NewLeaderObj, ReqUser.Id)) //Check if newLead is in higher position
                    {
                        Debug.WriteLine("Person: " + NewLeaderObj.UserName + " Became a leader of: " + ReqUser.UserName);
                        ReqUser.teamLead = NewLeaderObj;
                        UserManager.Update(ReqUser);
                        model.Employees.Remove(model.Employees.Find(x => x.Id == ReqUser.Id));
                    }
                    else if(TM.CheckIfLead(NewLeaderObj, ReqUser.Id)) //Check if newLead is in lower position
                    {
                        ModelState.AddModelError("", "Can't assign to the lower leader in the Hierarchy");
                    }
                }
                else ModelState.AddModelError("", "No such user exist");
                
                return View(model);
            }

            return View("Error"); //If you get here - you shouldn't be here
        }

    }
}