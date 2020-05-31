﻿using Microsoft.Ajax.Utilities;
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
                    Debug.WriteLine("Days in quarter: {0}", dayManager.getDaysInQuarterCount(viewModel.NewDayDate, viewModel.UserId));
                    Topic topic = new Topic();
                    if (viewModel.NewDayDate.Date < DateTime.Today)
                    {
                        ModelState.AddModelError("", "Cannot add a day to past.");
                    } else if (viewModel.NewDayTitle.IsNullOrWhiteSpace())
                    {
                        ModelState.AddModelError("", "Day title cannot be empty.");
                    } else if (viewModel.CreateTopic && viewModel.NewDayTitle.IsNullOrWhiteSpace())
                    {
                        ModelState.AddModelError("", "Topic Title cannot be empty.");
                    } else if (dayManager.getDaysInQuarterCount(viewModel.NewDayDate, viewModel.UserId) >= 3)
                    {
                        ModelState.AddModelError("", "Cannot add a day for, " + viewModel.NewDayDate + " - already have 3 learning days in the Quarter.");
                    } else if (dayManager.isDateTaken(viewModel.NewDayDate, viewModel.UserId))
                    {
                        ModelState.AddModelError("", "Cannot add a day for, " + viewModel.NewDayDate + " - date already has a learning day.");
                    }

                    if (ModelState.IsValid)
                    {
                        if (viewModel.CreateTopic)
                        { 
                            topic = topicManager.createTopic(viewModel.NewTopicTitle, viewModel.NewTopicDescription, viewModel.NewDayTopicId);
                        }
                        else
                        {
                            topic = topicManager.getTopicById(viewModel.NewDayTopicId);
                        }
                        dayManager.createLearningDay(viewModel.NewDayDate, viewModel.NewDayTitle, viewModel.NewDayDescription,
                        viewModel.UserId, new List<Topic>() { topic });

                        return RedirectToAction("Schedule");
                    } else
                    {
                        viewModel = getData(viewModel);
                        return View(viewModel);
                    }
                    
                }
                catch (Exception e)
                {
                    Debug.WriteLine("An error occurred while adding a new Learning day. LearningController ");
                    return View(getData(viewModel));
                }
            }
            else
            {
                LearningDay day = dayManager.getLearningDayById(int.Parse(viewModel.ViewedDayId));
                if (command == "Delete")
                {
                    if (day.Date.Date <= DateTime.Today.Date)
                    {
                        ModelState.AddModelError("", "Cannot delete a learning day that has started or is already over.");
                    }
                    else
                    {
                        dayManager.deleteLearningDay(day.LearningDayId, User.Identity.GetUserId());
                    }
                }
                else if (command == "Edit")
                {
                    if (day.Date.Date < DateTime.Today.Date)
                    {
                        ModelState.AddModelError("", "Cannot edit a learning day that is already over.");
                    }
                    else
                    {
                        return EditLearningDay(day.LearningDayId);
                    }
                }
            }
            
            return View(getData(viewModel));
        }

        [Authorize]
        // Get: /Learning/List/1234-abcd-...
        public ActionResult List(string id)
        {
            return RedirectToAction("List", "LearningDay", new { id = id });
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
            LearningDay editedDay = dayManager.getLearningDayById(id);
            EditLearningDayViewModel viewModel = new EditLearningDayViewModel();
            viewModel = getData(viewModel);
            viewModel.LearningDay = editedDay;
            return View("EditLearningDay", viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditLearningDay(EditLearningDayViewModel viewModel, string command)
        {
            viewModel = getData(viewModel);
            Debug.WriteLine("Edit learning day, command: " + command);

            string currentUserId = User.Identity.GetUserId();
            if (viewModel.LearningDay.User.Id != currentUserId) { 
                return View("Error"); // Cannot edit someone elses learning day.
            }

            if ((command == "Add Topic" || command == "Add New Topic") && viewModel.LearningDay.Topics.Where(tp => !tp.Remove).Count() >= 4)
            {
                ModelState.AddModelError("", "Maximum number (4) of topics per day reached.");
                return View(viewModel);
            }

            if (command == "Add Topic")
            {
                TopicDay topic = topicManager.createTopicDay(viewModel.AddTopicId, 
                    viewModel.LearningDay.LearningDayId,
                    viewModel.LearningDay.UserId);
                topic.NewlyCreated = true;
                // Forcefully loading the lazy Topic, so that it can be displayed.
                topic.Topic = topicManager.getTopicById(topic.TopicId);
                if (topic.Topic.ParentTopicId == null)
                {
                    topic.Topic.parentTopic = null;
                }
                else
                {
                    topic.Topic.parentTopic = topicManager.getTopicById((int)topic.Topic.ParentTopicId);
                }


                var duplicateTopicDays = viewModel.LearningDay.Topics.Where(topicDay => topicDay.TopicId == viewModel.AddTopicId);
                if (duplicateTopicDays.Count() > 0)
                {
                    // If the topic was marked for removal and is being added again, then it should just be visible again.
                    if (duplicateTopicDays.First().Remove)
                        duplicateTopicDays.First().Remove = false;
                    else
                        ModelState.AddModelError("", "The learning day already has this topic.");
                    return View(viewModel);
                }
                viewModel.LearningDay.Topics.Add(topic);
            } else if (command == "Add New Topic")
            {
                Topic newTopic = topicManager.createTopic(viewModel.NewTopicTitle, viewModel.NewTopicDescription, viewModel.NewTopicParentId);
                TopicDay topicDay = topicManager.createTopicDay(newTopic.TopicsId, viewModel.LearningDay.LearningDayId, viewModel.LearningDay.UserId);
                topicDay.NewlyCreated = true;
                topicDay.Topic = newTopic; // topicManager.getTopicById(newTopic.TopicsId);
                // Forcefully loading the lazy Topic, so that it can be displayed.
                //if (newTopic.ParentTopicId != null)
                //{
                //    topicDay.Topic.parentTopic = topicManager.getTopicById((int)newTopic.ParentTopicId);
                //}

                viewModel.LearningDay.Topics.Add(topicDay);
            }
            else if (command == "Add Reference")
            {
                LDayReferences reference = new LDayReferences() {
                    LearningDayId = viewModel.LearningDay.LearningDayId,
                    UserId = viewModel.LearningDay.UserId
                };
                viewModel.LearningDay.References.Add(reference);
            } else if (command == "Remove Selected Topics" || command == "Remove Selected References")
            {// Do nothing, as the objects are marked for deletion and remain in learning day until saved.

            } else if (command == "Cancel/Refresh")
            {
                viewModel.LearningDay = dayManager.getLearningDayById(viewModel.LearningDay.LearningDayId);
            } else
            {
                LearningDay oldDay = dayManager.getLearningDayById(viewModel.LearningDay.LearningDayId);
                if (viewModel.LearningDay.Date <= DateTime.Today.Date && viewModel.LearningDay.Date != oldDay.Date)
                {
                    ModelState.AddModelError("", "Cannot change learning day date to past date or today.");
                    return View(viewModel);
                } else if (!dayManager.canAddOrUpdateDay(viewModel.LearningDay))
                {
                    ModelState.AddModelError("", "Cannot change learning day date, maximum days (3) in target quarter reached.");
                    return View(viewModel);
                } else if (dayManager.isDateTaken(viewModel.LearningDay))
                {
                    ModelState.AddModelError("", "Cannot change learning day date, date already taken by another day.");
                    return View(viewModel);
                }


                if (!dayManager.updateLearningDay(viewModel.LearningDay, viewModel.LearningDay.RowVersion))
                {
                    TempData["UpdateLD"] = "The day was already modified by another user";
                    //LearningDay newDay = dayManager.getLearningDayById(learningDay.LearningDayId);
                    return RedirectToAction("EditLearningDay", new { id = viewModel.LearningDay.LearningDayId });
                }
                return RedirectToAction("Schedule", new { userId = viewModel.LearningDay.UserId });
            }

            return View(viewModel);
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

        [Authorize]
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

        private EditLearningDayViewModel getData(EditLearningDayViewModel model)
        {
            model.AllTopics = topicManager.getAllTopics();
            string currentUserId = User.Identity.GetUserId();
            model.RecommendedTopics = topicManager.getRecommendedTopics(currentUserId);
            return model;
        }
        private ScheduleViewModel getData(ScheduleViewModel model)
        {
            model.AllTopics = topicManager.getAllTopics();
            string currentUserId = User.Identity.GetUserId();
            model.UserId = currentUserId;
            model.RecommendedTopics = topicManager.getRecommendedTopics(currentUserId);
            return model;
        }
    }
}
