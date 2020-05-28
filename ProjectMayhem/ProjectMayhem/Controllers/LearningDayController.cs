using Microsoft.AspNet.Identity;
using ProjectMayhem.DbEntities;
using ProjectMayhem.Models;
using ProjectMayhem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectMayhem.Controllers
{
    public class LearningDayController : Controller
    {
        private LearningDayManager dayManager = new LearningDayManager();

        private List<LearningDay> _dayList;
        private List<LearningDay> DayList
        {
            get
            {
                if (_dayList == null)
                    _dayList = Session["DayList"] as List<LearningDay>;
                return _dayList;
            }
            set
            {
                _dayList = value;
                Session["DayList"] = _dayList;
            }
        }

        [Authorize]
        // GET: LearningDay/List/1234-ddas-12dk-ooo...
        public ActionResult List(string id)
        {
            if (DayList == null)
            {
                // TODO: load real data from database
                DayList = dayManager.getLearningDaysByUserId(id);
            }
            return View(DayList);
        }

        [Authorize]
        [HttpPost]
        public ActionResult List(List<LearningDay> dayList, string command)
        {
            try
            {
                if (command == "Remove Selected")
                {
                    int pos = dayList.Count();
                    while (pos > 0)
                    {
                        pos--;
                        if (dayList[pos].Remove)
                        {
                            dayManager.deleteLearningDay(dayList[pos].LearningDayId, User.Identity.GetUserId());
                            dayList.RemoveAt(pos);
                        }
                    }
                    DayList = dayList;
                }
                else if (command == "Cancel/Refresh")
                {
                    // force reload of data from database
                    DayList = null;
                }
                else
                {
                    // update actual database
                    if (dayList != null && dayList.Count > 0)
                    {
                        dayManager.UpdateChanges(dayList);
                    }
                    // force reload of data from database
                    DayList = null;
                }
                return RedirectToAction("List", new { id = User.Identity.GetUserId()});
            }
            catch
            {
                return View(dayList);
            }
        }
    }
}
