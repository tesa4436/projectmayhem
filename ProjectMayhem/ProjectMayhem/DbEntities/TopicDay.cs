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
        [Column(Order = 0)]
        [Key]
        public int LearningDayId { get; set; }
        [Column(Order = 1)]
        [Key]
        public string UserId { get; set; }
        [Column(Order = 2)]
        [Key]
        public int TopicId { get; set; }

        
        [ForeignKey("TopicId")]
        public virtual Topic Topic { get; set; }

        [ForeignKey("LearningDayId, UserId")]
        public virtual LearningDay Day { get; set; }

        [NotMapped]
        public bool Remove { get; set; }
        [NotMapped]
        public bool NewlyCreated { get; set; }
    }
}