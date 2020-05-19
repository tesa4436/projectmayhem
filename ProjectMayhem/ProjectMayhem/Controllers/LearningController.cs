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
        [AllowAnonymous]
        public ActionResult Schedule(string userId)
        {
            // Check if user is authorized to check the person's schedule.
            if (String.IsNullOrEmpty(userId) || User.Identity.GetUserId() == userId)
                userId = User.Identity.GetUserId();
            else
            {
                var ReqUser = UserManager.FindById(userId);
                if (!teamManager.CheckIfLead(ReqUser, User.Identity.GetUserId()))
                    return View("Error"); //If you get here you shouldnt be here
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

        // GET: Learning/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: Learning/AddLearningDay
        [HttpPost]
        public ActionResult AddLearningDay(ScheduleViewModel viewModel)
        {
            /*// Only the user can add a learning day for self.
            if (viewModel.UserId != User.Identity.GetUserId())
            {
                return View("Error");
            }
            */
            try
            {
                Debug.WriteLine("Adding a new learning day, date: {0}, title: {1}, description: {2}",
                    viewModel.NewDayDate, viewModel.NewDayTitle, viewModel.NewDayDescription);
                // TODO: Add insert logic here

                Topics topic = topicManager.getTopicById(viewModel.NewDayTopicId);
                dayManager.createLearningDay(viewModel.NewDayDate, viewModel.NewDayDescription, viewModel.UserId,
                    new List<Topics>() { topic });

                viewModel.NewDayDate = DateTime.Now;
                viewModel.NewDayDescription = "";
                viewModel.NewDayTitle = "";
                // To do: If there is no topic of a given name, then a new topic should be created.
                viewModel.NewDayTopicId = 1;
                return RedirectToAction("Schedule");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult EditLearningDay(int id)
        {
            Debug.WriteLine("Editing day: " + id);
            EditLearningDayViewModel viewModel = new EditLearningDayViewModel();
            LearningDay editedDay = dayManager.getLearningDayById(id);
            viewModel.Topics = editedDay.topics;
            viewModel.References = editedDay.references;
            viewModel.Date = editedDay.Date.ToString();
            viewModel.Description = editedDay.Description;
            viewModel.Title = "The title"; // editedDay.Title;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditLearningDay(EditLearningDayViewModel viewModel)
        {
            Debug.WriteLine("Updating learning day, date: {0}, title: {1}, description: {2}",
                    viewModel.Date, viewModel.Title, viewModel.Description);
            return RedirectToAction("Schedule");
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
            });

            return Content(list, "application/json");
        }

        // GET: Learning/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Learning/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }

  
}
