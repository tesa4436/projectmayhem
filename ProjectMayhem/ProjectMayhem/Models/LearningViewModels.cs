using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Models
{
    public class ScheduleViewModel
    {
        public int Quarter { get; set; }
        public int Year { get; set; }
        public List<LearningDay> LearningDays { get; set; }
        public ScheduleViewModel()
        {
            this.Year = DateTime.Now.Year;
            this.Quarter = DateTime.Now.Month / 3;
        }
    }

    public class AddLearningDayViewModel
    {
        [Required]
        public DateTime NewDate { get; set; }
        [Required]
        public string Topic { get; set; }
    }


}