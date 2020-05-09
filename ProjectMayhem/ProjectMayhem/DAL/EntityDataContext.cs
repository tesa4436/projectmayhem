using ProjectMayhem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ProjectMayhem.DAL
{
    public class EntityDataContext : DbContext
    {
        public EntityDataContext() : base("DefaultConnection")
        {
        }

        public DbSet<Topic> Topics { get; set; }
       

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}