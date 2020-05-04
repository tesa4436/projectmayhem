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
        public List<SubTopic> SubTopics;
    }

    public class SubTopic
    {
        public string Name;
    }

    public class LearningDay
    {
        public DateTime Date;
        public Topic Topic;
    }
}