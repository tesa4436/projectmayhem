using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProjectMayhem.DbEntities;
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
    public class RecommendationController : Controller
    {
       
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        TopicManager TopM = new TopicManager();
        // GET: Recommendation
        [Authorize]
        public ActionResult Recommended()
        {
            var model = new RecommendationViewModel();
            model = getData(model);
            return View(model);
        }

        private RecommendationViewModel getData(RecommendationViewModel model)
        {
            model.allTopics = TopM.getAllTopics();
            string currentUserId = User.Identity.GetUserId();
            model.TeamMembers = UserManager.Users.Where(x => x.teamLead.Id == currentUserId).ToList();
            model.myRecommendedTopics = TopM.getRecommendedTopics(currentUserId);
            return model;
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Recommended(RecommendationViewModel model)
        {
            model = getData(model);
            ModelState.Remove("newTopicParentId");
            if (UserManager.Users.Single(x => x.Id == model.selectedTeamMemberId) == null)
                ModelState.AddModelError("", "Team Member not found");

            if (ModelState.IsValid)
            {
                Debug.WriteLine("Parent id " +model.newTopicParentId);
                Debug.WriteLine("Testing recommend " + model.selectFromList);
                if (model.selectFromList == true)
                {
                    Debug.WriteLine("Testing recommend " + model.recommendedTopicId);
                    TopM.recommendTopic(model.recommendedTopicId, model.selectedTeamMemberId);
                }
                else
                {
                    Topic topic;
                    topic = TopM.createTopic(model.newTopicTitle, model.newTopicDescription, model.newTopicParentId);
                    TopM.recommendTopic(topic.TopicsId, model.selectedTeamMemberId);
                }
                return View(model);
            }
            return View(model);
        }
    }
}