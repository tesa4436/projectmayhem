using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Models
{
    public class MembersViewModel
    {
        public List<ApplicationUser> Employees { get; set; }


        [Display(Name = "New Leader")]
        [Required]
        public string NewLeadUserName { get; set; }

        [Display(Name = "Select a Team Member")]
        public string EmpId { get; set; }
    }
}