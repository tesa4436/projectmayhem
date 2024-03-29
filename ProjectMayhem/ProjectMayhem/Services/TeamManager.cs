﻿using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Microsoft.Owin.Security;
using System.Web;

namespace ProjectMayhem.Services
{
    public class TeamManager
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public List<ApplicationUser> GetMembersById(string teamLeadId)
        {
            List<ApplicationUser> returnValue = new List<ApplicationUser>();
            using (var context = new ApplicationDbContext())
            {
                var currentId = teamLeadId;
                var query = context.Users.Where(s => s.teamLead.Id == currentId);
                returnValue = query.ToList();
            }
            return returnValue;
        }

        public bool CheckIfLead(ApplicationUser user, string CurrentUserId)
        {
            using (var context = new ApplicationDbContext())
            {
                ApplicationUser reqUser;
                var query = context.Users.Where(s => s.Id == user.Id).Include(u => u.teamLead);
                reqUser = query.ToArray()[0];
                return CheckRecursion(reqUser, CurrentUserId);
            }
        }

        private bool CheckRecursion(ApplicationUser reqUser, string CurrentUserId)
        {
            if (reqUser.teamLead == null)
                return false;
            else if(reqUser.teamLead.Id == CurrentUserId)
                return true;
            else if(string.IsNullOrEmpty(reqUser.teamLead.Id))
                return false;
            else
                return CheckRecursion(reqUser.teamLead, CurrentUserId);
        }

        public string GetEmployeeIdByUsername(string UserName)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Users.Where(x => x.UserName == UserName);
                var user = query.ToArray()[0];
                System.Diagnostics.Debug.WriteLine("Found " + UserName + " whoose id is: " + user.Id);
                return user.Id;
            }
        }

    }
}