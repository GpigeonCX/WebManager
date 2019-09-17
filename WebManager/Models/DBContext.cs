using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebManager.Models
{
    public class DBContext : DbContext
    {

        public DBContext() : base("name=MyStrMssqlConn")
        {
        }
        public virtual DbSet<AgentReportModel> AgentReportModel { get; set; }
        public virtual DbSet<UserModel> UserModel { get; set; }
    }
}