using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class LearningController : Controller
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

        LearningDayManager dayManager = new LearningDayManager();
        TopicManager topicManager = new TopicManager();
        TeamManager teamManager = new TeamManager();

        // Learning schedule of a specific user.
        // GET: Learning/Schedule/123a-10df-...
        [Authorize]
        public ActionResult Schedule(string userId)
        {

            if (String.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();

            else
            {
                // If a user is not authorized to view the schedule, redirect to error page.
                if (!IsAuthorizedTo(Authorized.View, userId))
                    return View("Error");
            }
            
            List<LearningDay> learningDays = dayManager.getLearningDaysByUserId(userId);

            ScheduleViewModel viewModel = new ScheduleViewModel()
            {
                UserId = userId,
                AllTopics = topicManager.getAllTopics(),
                LearningDays = learningDays,
                Year = DateTime.Now.Year,
                Quarter = 1 + DateTime.Now.Month / 3
            };
            
            Debug.WriteLine("Schedule ViewModel initialized. UserId: {0}, LearnDays count: {1}", 
                userId, viewModel.LearningDays.Count);
            return View(viewModel);
        }

        // POST: Learning/Schedule
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Schedule(ScheduleViewModel viewModel, string command)
        {
            // User needs to be authorized to create learning days.
            Authorized allowedActions = CheckAuthorized(viewModel.UserId);
            if ((!HasAuthorization(allowedActions, Authorized.Create)))
            {
                return View("Error", new HandleErrorInfo(
                    new Exception("You may create/edit/remove learning days only for yourself."),
                    "Learning Controller",
                    "Schedule - Create learning day."));
            }
            if (command == "Add")
            {
                try
                {
                    Debug.WriteLine("Adding a new learning day, date: {0}, title: {1}, description: {2}, topicId: {3}",
                        viewModel.NewDayDate, viewModel.NewDayTitle, viewModel.NewDayDescription, viewModel.NewDayTopicId);
                    Debug.WriteLine("Target user: {0}", viewModel.UserId);

                    Topic topic = topicManager.getTopicById(viewModel.NewDayTopicId);
                    dayManager.createLearningDay(viewModel.NewDayDate, viewModel.NewDayTitle, viewModel.NewDayDescription,
                        viewModel.UserId, new List<Topic>() { topic });

                    return RedirectToAction("Schedule");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("An error occurred while adding a new Learning day. LearningController ");
                    return View();
                }
            } else if (command == "Delete")
            {
                dayManager.deleteLearningDay(int.Parse(viewModel.ViewedDayId), User.Identity.GetUserId());
            } else if (command == "Edit")
            {
                return EditLearningDay(int.Parse(viewModel.ViewedDayId));
            }

            return RedirectToAction("Schedule");
        }

        // Get: /Learning/EditLearningDay/1
        [Authorize]
        public ActionResult EditLearningDay(int id)
        {
            var error = (string)TempData["UpdateLD"];
            if (!string.IsNullOrEmpty(error))
            {
                ModelState.AddModelError("", error);
                TempData.Remove("UpdateLD");
            }
            Debug.WriteLine("Editing day: " + id);
            LearningDay editedDay = dayManager.getLearningDayById(id);
            Debug.WriteLine("Learning day values: " + " iD = " + editedDay.LearningDayId + " rowVersion = " + editedDay.RowVersion);
            return View("EditLearningDay", editedDay);
        }

        TopicManager TM;

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditLearningDay(LearningDay learningDay)
        {
            Debug.WriteLine("Updating learning day, date: {0}, title: {1}, description: {2}",
                    learningDay.Date, learningDay.Title, learningDay.Description);

            if (learningDay.UserId != User.Identity.GetUserId())
                return View("Error"); // Cannot edit someone elses learning day.
            
            //TM = new TopicManager();

            //learningDay.Topics.Add(new TopicDay() { TopicId = TM.createTopic("Testas123", "").TopicsId, LearningDayId = learningDay.LearningDayId, UserId = learningDay.UserId });
            //learningDay.Topics.Add(new TopicDay() { TopicId = TM.getTopicById(21).TopicsId, LearningDayId = learningDay.LearningDayId, UserId = learningDay.UserId });

            if (!dayManager.updateLearningDay(learningDay, learningDay.RowVersion))
            {
                TempData["UpdateLD"] = "The day was already modified by another user";
                //LearningDay newDay = dayManager.getLearningDayById(learningDay.LearningDayId);
                return RedirectToAction("EditLearningDay", new { id = learningDay.LearningDayId });
            }
            return RedirectToAction("Schedule");
        }

        //This class is used only for mapping data from ajax response
        public class StatusResponse
        {
            public List<int> ids { get; set; }
            public bool status { get; set; }
            public string userId { get; set; }
        }

        //Updating TopicUsers table here. Setting IsTopicLearned to true
        [HttpPost]
        public ActionResult SetTopicStatusToDone(StatusResponse resp)
        {
            Debug.WriteLine($"status: {resp.status}");
            
            TM = new TopicManager();
            foreach (var topicId in resp.ids)
            {
                TM.UpdateTopicUsersStatus(resp.userId, topicId, resp.status);
            }
  
            return Json(new { success = true, responseText = "Your message successfuly sent!" }, JsonRequestBehavior.AllowGet);
        }

        public ContentResult GetLearningDays(string id)
        {
            List<LearningDay> learningDays = dayManager.getLearningDaysByUserId(id);
            Debug.WriteLine("Getting {0} learning days as JSON for user {1}", learningDays.Count, id);
            var list = JsonConvert.SerializeObject(learningDays,
                Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
            );

            return Content(list, "application/json");
        }

        // Checks if the current user can perform actions with given user.
        private Authorized CheckAuthorized(string userId)
        {
            var rights = Authorized.None;
            var currentUserId = User.Identity.GetUserId();
            // If the user is editing personal information, grant all access.
            if (userId == currentUserId)
                return Authorized.View | Authorized.Edit | Authorized.Create | Authorized.Delete;
            // If the user is editing team members data, grant viewing access.
            var ReqUser = UserManager.FindById(userId);
            if (teamManager.CheckIfLead(ReqUser, User.Identity.GetUserId()))
                rights = Authorized.View;
            return rights;
        }

        // Returns true if a user is authorized to perform the specified action.
        private bool IsAuthorizedTo(Authorized action, string userId)
        {
            return (CheckAuthorized(userId) & action) != 0;
        }

        // Returns true if a given flag for authorized actions contains specific authorized action.
        // (Authorized.View | Authorized.Edit) & Authorized.View == Authorized.View
        private bool HasAuthorization(Authorized actions, Authorized action)
        {
            return (actions & action) == action;
        }
    }
}
