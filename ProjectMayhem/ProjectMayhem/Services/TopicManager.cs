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
        public ApplicationDbContext context;

        public Topic createTopic(string title, string description, int parentTopicId = -1)
        {
            using (context = new ApplicationDbContext())
            {
                var newT = new Topic();
                if (parentTopicId > 0)
                {
                    var parentTopic = context.topics.Where(x => x.TopicsId == parentTopicId).First();
                    if (parentTopic != null)
                        newT.parentTopic = parentTopic;
                }
                newT.Description = description;
                newT.Title = title;
                context.topics.Add(newT);
                context.SaveChanges();
                return newT;
            }
        }

        public void recommendTopic(int TopicId, string UserId)
        {
            using (context = new ApplicationDbContext())
            {
                var newTU = new TopicUser();
                var user = context.Users.Where(x => x.Id == UserId).First();
                if (user.RecommendedTopics.Where(x => x.TopicId == TopicId).ToArray().Length > 0)
                    return;
                var topic = context.topics.Where(x => x.TopicsId == TopicId).First();
                newTU.Topic = topic;
                newTU.User = user;
                context.topicUsers.Add(newTU);
                context.SaveChanges();
            }
        }

        public List<Topic> getRecommendedTopics(string UserId)
        {
            using (context = new ApplicationDbContext())
            {
                var RecommendedTopics = new List<Topic>();
                var TopicUsers = context.Users.Where(x => x.Id == UserId).First().RecommendedTopics;
                foreach(var topic in TopicUsers)
                {
                    RecommendedTopics.Add(topic.Topic);
                }
                return RecommendedTopics;
            }
        }

        public List<Topic> getAllTopics()
        {
            using (context = new ApplicationDbContext())
            {
                return context.topics.ToList();
            }
        }

        public Topic getTopicById(int Id)
        {
            using (context = new ApplicationDbContext())
            {
                return context.topics.Where(x => x.TopicsId == Id).First();
            }
        }

        public void UpdateTopicUsersStatus(string userId, int topicId, bool status)
        {
            using (context = new ApplicationDbContext())
            {
                var topicUser = context.topicUsers.Find(new object[] { userId, topicId });
                topicUser.IsTopicLearned = status;
                context.SaveChanges();
            }
        }

        public List<Topic> GetTopicsByTeam(string teamLeadId)
        {
            using (context = new ApplicationDbContext())
            {
                var topics = (from topic in context.topics
                              join tDay in context.topicDay on topic.TopicsId equals tDay.TopicId
                              join user in context.Users on tDay.UserId equals user.Id
                              where user.teamLead.Id == teamLeadId
                              select topic).ToList();
                /*var topics = (from user in context.Users
                             join learningDay in context.learningDays on user.Id equals learningDay.UserId
                             join topicDay in context.topicDay on learningDay.LearningDayId equals topicDay.LearningDayId
                              where user.teamLead.Id == teamLeadId
                              select topicDay.Topic).ToList();*/
                return topics;
            }
        }

    }
}