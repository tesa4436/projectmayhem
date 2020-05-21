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
            model.selectFromList = false;
            model = getRecommendationModelData(model);
            return View(model);
        }

        private RecommendationViewModel getRecommendationModelData(RecommendationViewModel model)
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
            model = getRecommendationModelData(model);
            ModelState.Remove("newTopicParentId");
            if (ModelState.IsValid)
            {
                if(model.selectFromList == true)
                {
                    TopM.recommendTopic(model.recommendedTopicId, model.selectedTeamMemberId);
                }
                else
                {
                    if(string.IsNullOrEmpty(model.newTopicTitle))
                    {
                        ModelState.AddModelError("", "Topic title cannot be empty");
                        return View(model);
                    }
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