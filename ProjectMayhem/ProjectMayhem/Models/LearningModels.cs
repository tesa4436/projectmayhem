using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Models
{
    public class Topic
    {
        public string name;
        public List<SubTopic> subTopics;
    }

    public class SubTopic
    {
        public string name;
    }

    public class LearningDay
    {
        public DateTime date;
        public Topic topic;
    }
}