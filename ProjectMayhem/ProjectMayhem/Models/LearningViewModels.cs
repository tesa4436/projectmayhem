using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public LearningDay ViewedDay { get; set; }
        public ScheduleViewModel()
        {
            this.Year = DateTime.Now.Year;
            this.Quarter = DateTime.Now.Month / 3;
        }
    }

    public class AddLearningDayViewModel
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TopicName { get; set; }
    }


}