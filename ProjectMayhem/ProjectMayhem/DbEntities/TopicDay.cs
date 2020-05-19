using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectMayhem.DbEntities
{
    public class TopicDay
    {

        [Key, Column(Order = 0)]
        [ForeignKey("Day")]
        public int DayId { get; set; }
        

        [Key, Column(Order = 1)]
        [ForeignKey("Topic")]
        public int TopicId { get; set; }

        public virtual Topic Topic { get; set; }

        public virtual LearningDay Day { get; set; }
    }
}