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
            Topics = new List<TopicDay>();
            References = new List<LDayReferences>();
        }

        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LearningDayId { get; set; }

        [Key, Column(Order = 1), ForeignKey("User")]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public virtual ApplicationUser User {get; set;}

        //Does not exist in DB
        public virtual ICollection<TopicDay> Topics { get; set; }

        public virtual ICollection<LDayReferences> References { get; set; }
    }
}