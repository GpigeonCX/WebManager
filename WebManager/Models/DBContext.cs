using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebManager.Models
{
    public class DBContext : DbContext
    {

        static DBContext()
        {
            Database.SetInitializer<DBContext>(null);
        }
        public DBContext() : base("name=MyStrMssqlConn")
        {
            //Database.SetInitializer<PortalContext>(null);
        }
        public virtual DbSet<AgentReportModel> AgentReportModel { get; set; }
        public virtual DbSet<UserModel> UserModel { get; set; }
    }
}