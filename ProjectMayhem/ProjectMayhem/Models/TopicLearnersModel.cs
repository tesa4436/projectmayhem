using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectMayhem.Models
{
    public class TopicLearnersModel
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }

}
}