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
        [Key]
        [ForeignKey("learningDay")]
        [Column(Order = 1)]
        public int LDId { get; set; }

        public virtual LearningDay learningDay { get; set; }

        [Key]
        [Column(Order = 0)]
        public string ReferenceUrl { get; set; }
    }
}