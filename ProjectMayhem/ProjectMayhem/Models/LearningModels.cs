using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Models
{
    // End goal - DB entities and using code-first approach.
    public class Topic
    {
        public string Name { get; set; }
        public Topic ParentTopic;
        public List<string> References;
    }

    public class LearningDay
    {
        public DateTime Date;
        public List<Topic> Topics;
        public string[] References;
        public string Description;
        public string Title;
    }
}