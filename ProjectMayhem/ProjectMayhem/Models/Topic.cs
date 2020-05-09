using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Models
{
    public class Topic
    {
        public int TopicID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public Topic ParentID { get; set; }
    }
}