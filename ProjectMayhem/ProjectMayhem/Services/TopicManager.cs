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

        public List<Topic> getAllTopics()
        {
            return applicationDbContext.topics.ToList();
        }

        public Topic getTopicById(int id)
        {
            return applicationDbContext.topics.Where(x => x.TopicsId == id).First();
        }

    }
}