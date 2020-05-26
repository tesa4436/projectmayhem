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

        public TopicDay createTopicDay(int topicId, int learningDayId, string userId)
        {
            return new TopicDay()
            {
                TopicId = getTopicById(topicId).TopicsId,
                LearningDayId = learningDayId,
                UserId = userId
            };
        }

        public TopicDay createDayForNewTopic(int dayId, string userId, string topicTitle, string topicDescription, int parentId = -1)
        {
            return new TopicDay()
            {
                TopicId = createTopic(topicTitle, topicDescription, parentId).TopicsId,
                LearningDayId = dayId,
                UserId = userId
            };
        }
    }
}