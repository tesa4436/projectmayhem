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

        // GET: Learning/Calendar
        public ActionResult Calendar()
        {
            return View();
        }

        // GET: Learning/Create
        public ActionResult AddLearningDay()
        {
            AddLearningDayViewModel addLearningDayViewModel = new AddLearningDayViewModel();
            return View(addLearningDayViewModel);
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
                    date = viewModel.NewDate,
                    topic = new Topic { name = viewModel.Topic }
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
