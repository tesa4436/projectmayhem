using ProjectMayhem.DbEntities;
using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
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
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var user = context.Users.Where(x => x.Id == userId).First();
                        var LD = new LearningDay();
                        LD.Date = date; LD.Description = Desc; LD.User = user; LD.Title = Title;
                        LD = context.learningDays.Add(LD);
                        context.SaveChanges();
                        if (chosenTopics != null)
                        {
                            foreach (var topic in chosenTopics)
                            {
                                context.topicDay.Add(new TopicDay() { Day = LD, Topic = context.topics.Where(x => x.TopicsId == topic.TopicsId).First() });
                            }
                        }
                        if (references != null)
                        {
                            foreach (var reference in references)
                            {
                                context.lDayReferences.Add(new LDayReferences() { learningDay = LD, ReferenceUrl = reference });
                            }
                        }

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                
            }
        }

        public bool updateLearningDay(LearningDay changedDay, byte[] RowState)
        {
            using (var context = new ApplicationDbContext())
            {
                if (context.topicDay.Where(x => x.LearningDayId == changedDay.LearningDayId).ToArray().Length == 0) 
                    return false;
                try
                {
                    var update = context.learningDays.Single(x => x.LearningDayId == changedDay.LearningDayId);
                    update.Date = changedDay.Date;
                    update.Title = changedDay.Title;
                    update.Description = changedDay.Description;
                    context.Entry(update).OriginalValues["RowVersion"] = RowState;
                    foreach(var topic in changedDay.Topics)
                    {
                        if (topic.Remove)
                        {
                            TopicDay toRemove = context.topicDay.Single(x => x.TopicId == topic.TopicId
                                && x.LearningDayId == topic.LearningDayId
                                && x.UserId == topic.UserId);
                            context.topicDay.Remove(toRemove);
                        }
                        else
                        {
                            context.topicDay.AddOrUpdate(topic);
                        }
                    }
                    foreach (var reference in changedDay.References)
                    {
                        if (reference.Remove)
                        {
                            LDayReferences toRemove = context.lDayReferences.First(x => x.LearningDayId == reference.LearningDayId
                                && x.UserId == reference.UserId
                                && x.ReferenceUrl == reference.ReferenceUrl);
                            context.lDayReferences.Remove(toRemove);
                        }
                        else
                        {
                            context.lDayReferences.AddOrUpdate(reference);
                        }
                        
                    }
                    context.learningDays.AddOrUpdate(update);
                    context.SaveChanges();
                    return true;
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    return false;
                }
                catch(DbUpdateException ex)
                {
                    Exception tempEx = ex;
                    // Exception may be nested. Might need to go deep for the exception message.
                    while (tempEx.InnerException != null)
                    {
                        tempEx = tempEx.InnerException;
                    }
                    Debug.WriteLine(tempEx.Message);
                    return false;
                }
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

        public int getDaysInQuarterCount(DateTime date, string UserId)
        {
            // 1st quarter: 1, 2, 3 months, 2nd: 4, 5, 6, 3rd: 7, 8, 9, 4th: 10, 11, 12.
            int quarter = (date.Month - 1) / 3;
            DateTime quarterStart = new DateTime(date.Year, 1 + quarter * 3, 1),
                quarterEnd = new DateTime(date.Year, 4 + quarter * 3, 1);

            Debug.WriteLine("Counting learning days within: {0}-{1}", quarterStart.ToString(), quarterEnd.ToString());
            using (var context = new ApplicationDbContext())
            {
                return context.learningDays.Where(x => x.UserId == UserId && x.Date >= quarterStart && x.Date < quarterEnd).ToList().Count;
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

        public void UpdateChanges(List<LearningDay> learningDays)
        {
            foreach(var day in learningDays)
            {
                updateLearningDay(day, day.RowVersion);
            }
            applicationDbContext.SaveChanges();
        }

        public void UpdateChanges()
        {
            applicationDbContext.SaveChanges();
        }

    }
}