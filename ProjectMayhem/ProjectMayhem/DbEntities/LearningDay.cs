using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectMayhem.DbEntities
{
    public class LearningDay
    {
        public LearningDay()
        {
            topics = new List<TopicDay>();
            references = new List<LDayReferences>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LearningDayId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public string Description { get; set; }

        public virtual ApplicationUser User {get; set;}

        public virtual ICollection<TopicDay> topics { get; set; }

        public virtual ICollection<LDayReferences> references { get; set; }
    }
}