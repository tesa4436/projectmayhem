using ProjectMayhem.DbEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Models
{
    public class RecommendationViewModel
    {
        [Display(Name ="Select Topic from list instead")]
        public bool selectFromList { get; set; }

        [Display(Name ="New Topic title")]
        public string newTopicTitle { get; set; }

        [Display(Name ="New Topic Description")]
        public string newTopicDescription { get; set; }

        [Display(Name ="Select a parent Topic (if there is one)")]
        public int newTopicParentId { get; set; }

        public List<ApplicationUser> TeamMembers { get; set; }
        [Display(Name ="Select a Team Member to recommend the Topic")]
        public string selectedTeamMemberId { get; set; }

        public List<Topic> allTopics { get; set; }

        public List<Topic> myRecommendedTopics { get; set; }
        
        [Display(Name ="Select a Topic to recommend")]
        public int recommendedTopicId { get; set; }
    }
}