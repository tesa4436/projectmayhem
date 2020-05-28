using ProjectMayhem.DbEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Models
{
    // May also be used to add a new learning day.
    public class ScheduleViewModel : AddLearningDayViewModel
    {
        public int Quarter { get; set; }
        public int Year { get; set; }
        public List<LearningDay> LearningDays { get; set; }

        // Custom setters and getters so that ViewedDay changes whenever ViewedDayId changes due to data binding.
        // This doesn't work.
        private string viewedDayId;
        [Display(Name = "Learning day ID")]
        public string ViewedDayId {
            get { return this.viewedDayId; } 
            set { 
                this.viewedDayId = value;
                // this.ViewedDay = this.LearningDays.Find(day => day.Id == this.viewedDayId);
                // Debug.WriteLine("ViewedDayId changed to: {0}. ViewedDay found: {1}",this.viewedDayId, ViewedDay != null);
            } 
        }
    }

    public class AddLearningDayViewModel
    {
        public List<Topic> AllTopics { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime NewDayDate { get; set; }
        [Display(Name = "Title")]
        public string NewDayTitle { get; set; }
        [Display(Name = "Description")]
        public string NewDayDescription { get; set; }
        [Display(Name = "Select your topic")]
        public int NewDayTopicId { get; set; }

        // If creating a new topic:

        [Display(Name = "Create a new topic")]
        public bool CreateTopic { get; set; }

        [Display(Name = "Title")]
        public string NewTopicTitle { get; set; }
        [Display(Name = "Parent topic")]
        public string NewTopicParentId { get; set; }
        [Display(Name = "Description")]
        public string NewTopicDescription { get; set; }

        // Hidden field.
        public string UserId { get; set; }
    }

    public class EditLearningDayViewModel
    {
        public ICollection<LDayReferences> References { get; set; }
        
        public ICollection<TopicDay> Topics { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int LearningDayId { get; set; }
    }


}