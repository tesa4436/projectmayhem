using ProjectMayhem.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Models
{
    public class ReportViewModel
    {
        public List<Topic> Topics { get; set; }

        public Topic SelectedTopic { get; set; }

        public List<ApplicationUser> Users { get; set; }
    }
}