using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectMayhem.Controllers
{
    public class PersonListController : Controller
    {
        // TODO: remove mock database
        private static List<Models.Person> MockDb = new List<Models.Person>
            {
                new Models.Person { PrimaryKey = 1, FirstName = "Fred", LastName="Than", Age=54 },
                new Models.Person { PrimaryKey = 2, FirstName = "Erin", LastName="Saavedra", Age=37 },
                new Models.Person { PrimaryKey = 4, FirstName = "Abdul", LastName = "Banas", Age = 12 }
            };

        private List<Models.Person> _personList;
        private List<Models.Person> PersonList
        {
            get
            {
                if (_personList == null)
                    _personList = Session["PersonList"] as List<Models.Person>;
                return _personList;
            }
            set
            {
                _personList = value;
                Session["PersonList"] = _personList;
            }
        }

        // GET: PersonList
        public ActionResult Index()
        {
            if (PersonList == null)
            {
                // TODO: load real data from database
                PersonList = MockDb;
            }
            return View(PersonList);
        }

        [HttpPost]
        public ActionResult Index(List<Models.Person> personList, string command)
        {
            try
            {
                if (command == "Add Item")
                {
                    personList.Add(new Models.Person { PrimaryKey = -1 });
                    PersonList = personList;
                }
                else if (command == "Remove Selected")
                {
                    int pos = personList.Count();
                    while (pos > 0)
                    {
                        pos--;
                        if (personList[pos].Remove)
                            personList.RemoveAt(pos);
                    }
                    PersonList = personList;
                }
                else if (command == "Cancel/Refresh")
                {
                    // force reload of data from database
                    PersonList = null;
                }
                else
                {
                    // update actual database
                    MockDb = personList;
                    // force reload of data from database
                    PersonList = null;
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
