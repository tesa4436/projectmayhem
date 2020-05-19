using ProjectMayhem.DbEntities;
using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Services
{
    public class TopicManager
    {
        ApplicationDbContext applicationDbContext = new ApplicationDbContext();

        public List<Topics> getAllTopics()
        {
            return applicationDbContext.topics.ToList();
        }

        public Topics getTopicById(int id)
        {
            return applicationDbContext.topics.Where(x => x.TopicsId == id).First();
        }

    }
}