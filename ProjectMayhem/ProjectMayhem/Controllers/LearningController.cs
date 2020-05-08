using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectMayhem.Controllers
{
    public class LearningController : Controller
    {
        // GET: Learning/Schedule
        [AllowAnonymous]
        public ActionResult Schedule()
        {
            ScheduleViewModel viewModel = new ScheduleViewModel();
            // Could add learning days from database here.
            // viewModel.LearningDays = 

            return View(viewModel);
        }

        // GET: Learning/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Learning/Create
        public ActionResult AddLearningDay()
        {
            AddLearningDayViewModel addLearningDayViewModel = new AddLearningDayViewModel();
            return View(addLearningDayViewModel);
        }

        public JsonResult GetLearningDays()
        {
            List<LearningDay> days = new List<LearningDay>();
            string[] references = { "https://google.com", "https://wikipedia.org", "https://stackoverflow.com/" };
            List<Topic> topics = new List<Topic> { new Topic { Name = "Some body touched my baguette!" } ,
            new Topic { Name = "MSSQL basics" }, new Topic { Name = "Yeah, aha, you know what it is." } };

            days.Add(new LearningDay { Date = DateTime.Now, References = references, 
                Description = "Assembly, Basic, C#",
                Title = "Programming ABC",
                Topics = topics });
            days.Add(new LearningDay { Date = DateTime.Now.AddDays(1), References = references,
                Description = "Learning stuff - good. Hehe.",
                Title = "Programming AB", 
                Topics = topics });
            days.Add(new LearningDay { Date = DateTime.Now.AddDays(3), References = references,
                Description = "Everything I do, I do it big. Yeah, uh huh, screamin' that's nothin'",
                Title = "Black and yellow",
                Topics = new List<Topic> { new Topic { Name = "Yeah, aha, you know what it is." } } });
            return new JsonResult { Data = days, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        // POST: Learning/Create
        [HttpPost]
        public ActionResult Create(AddLearningDayViewModel viewModel)
        {
            try
            {
                // TODO: Add insert logic here
                LearningDay newDay = new LearningDay
                {
                    Date = viewModel.NewDate,
                    Topics = new List<Topic> { new Topic { Name = viewModel.Topic } }
                };

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Learning/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Learning/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Learning/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Learning/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
