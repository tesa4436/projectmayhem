using ProjectMayhem.DbEntities;
using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Services
{
    public class UserManager
    {
        public ApplicationDbContext context;

        public List<Topic> GetLearnedTopicsByUser(string userId)
        {
            using (context = new ApplicationDbContext())
            {
                var topics = (from topic in context.topics
                              join topicUsr in context.topicUsers on topic.TopicsId equals topicUsr.TopicId
                              where topicUsr.UserId == userId && topicUsr.IsTopicLearned == true
                              select topic).ToList();

                return topics;     
            }
        }

        public List<ApplicationUser> GetUserByLearnedTopic(int topicId)
        {
            using (context = new ApplicationDbContext())
            {
                var users = (from user in context.Users
                              join topicUsr in context.topicUsers on user.Id equals topicUsr.UserId
                              where topicUsr.TopicId == topicId && topicUsr.IsTopicLearned == true
                              select user).ToList();

                return users;
            }
        }
    }
}