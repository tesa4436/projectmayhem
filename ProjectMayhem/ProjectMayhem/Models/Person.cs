// Goes in Models
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Models
{
    public class Person
    {
        public int PrimaryKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public bool Remove { get; set; }
    }
}