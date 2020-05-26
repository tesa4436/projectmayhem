using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectMayhem.DbEntities
{
    public class TopicUser
    {
        [Key, Column(Order = 0)]
        [ForeignKey("User")]
        public string UserId { get; set; }


        [Key, Column(Order = 1)]
        [ForeignKey("Topic")]
        public int TopicId { get; set; }

        public virtual Topic Topic { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}