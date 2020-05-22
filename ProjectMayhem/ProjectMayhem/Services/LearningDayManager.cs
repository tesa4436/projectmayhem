using ProjectMayhem.DbEntities;
using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Services
{
    public class LearningDayManager
    {

        ApplicationDbContext applicationDbContext = new ApplicationDbContext();

        public List<LearningDay> getLearningDaysByUserId(string userId)
        {
            return applicationDbContext.learningDays.Where(x => x.User.Id == userId).ToList();
        }

        public LearningDay getLearningDayByDate(string userId, DateTime date)
        {
            return applicationDbContext.learningDays.Where(x => x.User.Id == userId && x.Date == date).First();
        }

        public LearningDay getLearningDayById(int dayId)
        {
            return applicationDbContext.learningDays.Where(x => x.LearningDayId == dayId).First();
        }

        public bool createLearningDay(DateTime date, string Title, string Desc, string userId, List<Topic> chosenTopics = null, List<string> references = null)
        {
            using (var context = new ApplicationDbContext()) {
                var user = context.Users.Where(x => x.Id == userId).First();
                var LD = new LearningDay();
                LD.Date = date; LD.Description = Desc; LD.User = user; LD.Title = Title;
                if (chosenTopics != null)
                {
                    foreach (var topic in chosenTopics)
                    {
                        LD.Topics.Add(new TopicDay() { Day = LD, Topic = context.topics.Where(x => x.TopicsId == topic.TopicsId).First() });
                    
                    }
                }
                if(references != null)
                {
                    foreach(var reference in references)
                    {
                        LD.References.Add(new LDayReferences() { learningDay = LD, ReferenceUrl = reference });
                    }
                }
                context.learningDays.Add(LD);
                context.SaveChanges();
                return true;
            }
        }

        public bool updateLearningDay(LearningDay changedDay)
        {
            if (changedDay.Topics.Count == 0)
            {
                Debug.WriteLine("Failed to update learning day, it has no topics");
                return false;
            }
            using (var context = new ApplicationDbContext())
            {
                LearningDay oldDay = getLearningDayById(changedDay.LearningDayId);
                context.learningDays.AddOrUpdate(changedDay);
                context.SaveChanges();
                return true;
            }
        }

        public void deleteLearningDay(DateTime dateTime, string UserId)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.Where(x => x.Id == UserId).First();
                context.learningDays.Remove(user.LearningDays.Where(x => x.Date == dateTime).First());
                context.SaveChanges();
            }
        }

        public void deleteLearningDay(int dayId, string UserId)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.Where(x => x.Id == UserId).First();
                context.learningDays.Remove(user.LearningDays.Where(x => x.LearningDayId == dayId).First());
                context.SaveChanges();
            }
        }

        public void setTopics(LearningDay learningDay, List<Topic> newTopics)
        {
            using (var context = new ApplicationDbContext())
            {
                if (newTopics != null)
                {
                    var NewTopicDays = new List<TopicDay>();
                    foreach(var topic in newTopics)
                    {
                        NewTopicDays.Add(new TopicDay() { Day = learningDay, Topic = topic });
                    }
                    learningDay.Topics = NewTopicDays;
                    UpdateChanges();
                }
            }
        }

        public Topic getTopicById(int id)
        {
            return applicationDbContext.topics.Where(x => x.TopicsId == id).First();
        }

        public void setReferences(LearningDay learningDay, List<string> references)
        {
            using (var context = new ApplicationDbContext())
            {
                if (references != null)
                {
                    var newReferenceDays = new List<LDayReferences>();
                    foreach (var reference in references)
                    {
                        newReferenceDays.Add(new LDayReferences() { learningDay = learningDay, ReferenceUrl = reference });
                    }
                    learningDay.References = newReferenceDays;
                    UpdateChanges();
                }
            }
        }

        public void UpdateChanges()
        {
            applicationDbContext.SaveChanges();
        }

    }
}