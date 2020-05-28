using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectMayhem.DbEntities
{
    public class LDayReferences
    {
        [Key, Column(Order = 0)]
        public int LearningDayId { get; set; }
        
        [Key, Column(Order = 1)]
        public string UserId { get; set; }
        
        
        [ForeignKey("LearningDayId, UserId")]
        public virtual LearningDay learningDay { get; set; }

        [Key]
        [Column(Order = 2)]
        public string ReferenceUrl { get; set; }
    }
}