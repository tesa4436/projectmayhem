﻿using ProjectMayhem.DbEntities;
using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
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

        public bool createLearningDay(DateTime date, string Desc, string userId, List<Topics> chosenTopics = null, List<string> references = null)
        {
            using (var context = new ApplicationDbContext()) {
                var user = context.Users.Where(x => x.Id == userId).First();
                var LD = new LearningDay();
                LD.Date = date; LD.Description = Desc; LD.User = user;
                if (chosenTopics != null)
                {
                    foreach (var topic in chosenTopics)
                    {
                        LD.topics.Add(new TopicDay() { Day = LD, Topic = context.topics.Where(x => x.TopicsId == topic.TopicsId).First() });
                    
                    }
                }
                if(references != null)
                {
                    foreach(var reference in references)
                    {
                        LD.references.Add(new LDayReferences() { learningDay = LD, ReferenceUrl = reference });
                    }
                }
                context.learningDays.Add(LD);
                context.SaveChanges();
                return true;
            }
        }

        public void setTopics(LearningDay learningDay, List<Topics> newTopics)
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
                    learningDay.topics = NewTopicDays;
                    UpdateChanges();
                }
            }
        }

        public Topics getTopicById(int id)
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
                    learningDay.references = newReferenceDays;
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