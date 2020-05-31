using ProjectMayhem.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectMayhem.Models
{
    public class ReportViewModel
    {
        public List<Topic> Topics { get; set; }

        public string SelectedTopicId { get; set; }

        public List<ApplicationUser> Users { get; set; }
    }
}