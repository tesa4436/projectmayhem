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
            
            if (String.IsNullOrEmpty(Id) || User.Identity.GetUserId() == Id)
                Id = User.Identity.GetUserId();
            else
            {
                var ReqUser = UserManager.FindById(Id);
                if (!TM.CheckIfLead(ReqUser, User.Identity.GetUserId()))
                    return View("Error"); //If you get here you shouldnt be here
            }

            MembersViewModel viewModel = new MembersViewModel();
            ViewBag.Title = "Members of " + UserManager.FindById(Id).UserName +"'s team:";
            viewModel.Employees = UserManager.Users.Where(x => x.teamLead.Id == Id).ToList();//TM.GetMembersById(Id);
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
            ApplicationUser CurentTeamLead;

            if (!string.IsNullOrEmpty(EmpId) && User.Identity.GetUserId() != EmpId)
                CurentTeamLead = await UserManager.FindByIdAsync(EmpId);
            else
                CurentTeamLead = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            ViewBag.Title = "Members of " + CurentTeamLead.UserName + "'s team:";
            model.Employees = UserManager.Users.Where(x => x.teamLead.Id == CurentTeamLead.Id).ToList();//TM.GetMembersById(CurentTeamLead.Id);

            Debug.WriteLine("Selected team Member is " + model.EmpId);
            Debug.WriteLine("Possible lead is " + model.NewLeadId);

            if (string.IsNullOrEmpty(model.EmpId))
                ModelState.AddModelError("", "Team Member not selected");
            else if (string.IsNullOrEmpty(model.NewLeadId))
                ModelState.AddModelError("", "Leader Not selected");
            else if (CurentTeamLead.Id == model.NewLeadId)
                ModelState.AddModelError("", "The Member is in that team");
            else if(model.NewLeadId == model.EmpId)
                ModelState.AddModelError("", "Cant assign to himself");

            if (ModelState.IsValid)
            {
                var ReassignedMember = await UserManager.FindByIdAsync(model.EmpId);

                if (TM.CheckIfLead(ReassignedMember, User.Identity.GetUserId()))  //Checking if you are a higher leader
                {
                    var newLead = model.NewLeadId;    // Got an employee Id by his username
                        
                    if (newLead != null)
                    {                                        // Checking if newLead is legit
                        var NewLeaderObj = await UserManager.FindByIdAsync(newLead);
                        if (TM.CheckIfLead(ReassignedMember, NewLeaderObj.Id) || !TM.CheckIfLead(NewLeaderObj, ReassignedMember.Id)) //Check if newLead is in higher position
                        {
                            Debug.WriteLine("Person: " + NewLeaderObj.UserName + " Became a leader of: " + ReassignedMember.UserName);
                            ReassignedMember.teamLead = NewLeaderObj;
                            UserManager.Update(ReassignedMember);
                            model.Employees.Remove(model.Employees.Find(x => x.Id == ReassignedMember.Id));
                        }
                        else if (TM.CheckIfLead(NewLeaderObj, ReassignedMember.Id)) //Check if newLead is in lower position
                        {
                            ModelState.AddModelError("", "Can't assign to the lower leader in the Hierarchy");
                        }
                    }
                    else ModelState.AddModelError("", "No such user exist");

                    return View(model);
                }
                return View("Error"); //If you get here - you shouldn't be in this view at all
            }
            else
                return View(model);
        }
    }
}