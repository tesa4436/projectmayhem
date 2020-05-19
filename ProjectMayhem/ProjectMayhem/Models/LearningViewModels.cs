﻿using System;
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
        public List<Topic> AllTopics { get; set; }
        public LearningDay ViewedDay { get; set; }

        // Custom setters and getters so that ViewedDay changes whenever ViewedDayId changes due to data binding.
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
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Display(Name = "Select your topic")]
        public string TopicName { get; set; }
    }

    public class EditLearningDayViewModel
    {
        public string[] References { get; set; }
        
        public List<Topic> Topics { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public string Date { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
    }


}