using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectMayhem.DbEntities
{
    public class Topics
    {
        public Topics()
        {
            Days = new List<TopicDay>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TopicsId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public virtual ICollection<TopicDay> Days { get; set; }
    }
}