using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebManager.Models;

namespace WebManager.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        readonly DBContext Db = new DBContext();
        private static readonly string UserAdmin = ConfigurationManager.ConnectionStrings["admin"].ConnectionString;

        public ActionResult GetEmployee()

        {
            try
            {
                int pageIndex = Request["page"] == null ? 1 : int.Parse(Request["page"]);
                DBContext Db1 = new DBContext();
                int pageSize = Request["limit"] == null ? 50 : int.Parse(Request["limit"]);
                var OperatePerson = Request["OperatePerson"] ?? "";
                if (OperatePerson.Equals("") || !GetOperatePerson(OperatePerson))
                    return Json(null, JsonRequestBehavior.AllowGet);
                var data1 = from r in Db.AgentReportModel.Where(o => o.Flag == true).ToList()
                            select new
                            {
                                r.ID,
                                r.OperatePerson,
                                r.CardID,
                                StartDate = r.StartDate.ToString("d"),
                                StartTime = r.StartTime.ToString("t"),
                                EndTime = r.EndTime.ToString("t"),
                                r.ClockPlan,
                                EndDate = r.EndDate.ToString("d"),
                            };
                if (!OperatePerson.Equals(UserAdmin))
                    data1 = data1.Where(r => r.OperatePerson.Equals(OperatePerson.Trim()));
                int total = data1.Count();//总条数
                                          //构造成Json的格式传递
                var result = new { code = 0, msg = "123", count = total, data = data1.ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize) };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult GetUsers()
        {
            try
            {
                int pageIndex = Request["page"] == null ? 1 : int.Parse(Request["page"]);
                DBContext Db1 = new DBContext();
                int pageSize = Request["limit"] == null ? 50 : int.Parse(Request["limit"]);
                var OperatePerson = Request["OperatePerson"] ?? "";
                if (!OperatePerson.Equals(UserAdmin))
                    return null;
                var data1 = from r in Db.UserModel.Where(o => o.Flag == true).ToList()
                            select new
                            {
                                r.ID,
                                r.UserName,
                                CreateTime = r.CreateTime.ToString(),
                            };
                int total = data1.Count();//总条数
                                          //构造成Json的格式传递
                var result = new { code = 0, msg = "123", count = total, data = data1.ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize) };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult AddUser()
        {
            try
            {
                var OperatePerson = Request["OperatePerson"] ?? "";
                var AddOperatePerson = Request["addOperatePerson"] ?? "";
                if (AddOperatePerson.Equals("") || OperatePerson.Equals("") || !OperatePerson.Equals(UserAdmin))
                {
                    return Json("请检查登入账号,只有管理员能添加代理账号！");
                }
                UserModel model = new UserModel
                {
                    ID = Guid.NewGuid(),
                    UserName = AddOperatePerson,
                    UserPwd = "",
                    CreateTime = DateTime.Now,
                    LastModifyTime = DateTime.Now,
                    Flag = true,
                };
                Db.UserModel.Add(model);
                Db.Entry<UserModel>(model).State = EntityState.Added;
                if (Db.SaveChanges() > 0)
                {
                    return Json("OK");
                }
                else
                {
                    return Json("添加失败");
                }
            }
            catch (Exception ex)
            {
                return Json($"添加失败,请检查数据格式！异常信息：{ex}");
            }
        }
        [HttpPost]
        public ActionResult Add()
        {
            try
            {
                var OperatePerson = Request["OperatePerson"] ?? "";
                if (OperatePerson.Equals("") || !GetOperatePerson(OperatePerson))
                {
                    return Json("请检查登入账号！");
                }
                string[] AddCardIds = Request["AddCardId"].ToString().Split(new char[] { '，', ',' }, StringSplitOptions.RemoveEmptyEntries);
                var AddPassWord = Request["AddPassWord"] ?? "";
                var AddEndDate = Convert.ToDateTime(Request["AddEndDate"]);
                var AddStartTime = Convert.ToDateTime(Request["AddStartTime"]);
                var AddEndTime = Convert.ToDateTime(Request["AddEndTime"]);
                var AddClockPlan = Convert.ToInt32(Request["AddClockPlan"]);
                foreach (var AddCardId in AddCardIds)
                {
                    AgentReportModel model = new AgentReportModel
                    {
                        ID = Guid.NewGuid(),
                        UserName = "",
                        CardID = AddCardId,
                        PassWord = AddPassWord,
                        StartDate = DateTime.Now,
                        EndDate = AddEndDate,
                        ClockPlan = AddClockPlan,
                        StartTime = AddStartTime,
                        EndTime = AddEndTime,
                        NeedClock = true,
                        Ratio = 1,
                        OperatePerson = OperatePerson,
                        CreateTime = DateTime.Now,
                        LastModifyTime = DateTime.Now,
                        Flag = true,
                    };
                    Db.AgentReportModel.Add(model);
                    Db.Entry<AgentReportModel>(model).State = EntityState.Added;
                }
                if (Db.SaveChanges() > 0)
                {
                    return Json("OK");
                }
                else
                {
                    return Json("添加失败");
                }
            }
            catch (Exception ex)
            {
                return Json($"添加失败,请检查数据格式！异常信息：{ex}");
            }

        }
        public ActionResult ExportFileExcel()
        {
            var OperatePerson = Request["OperatePerson"] ?? "";
            if (OperatePerson.Equals("") || !GetOperatePerson(OperatePerson))
            {
                return Json("请检查登入账号！");
            }
            string id = Request["Ids"];
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //List<AgentReportModel> data = Db.AgentReportModel
            //                            .Where(o => o.OperatePerson.Equals(OperatePerson))
            //                            .Where(x => ids.Contains(x.ID.ToString())).ToList();

            var data = from r in Db.AgentReportModel.Where(x => ids.Contains(x.ID.ToString())).ToList()
                       select new
                       {
                           r.CardID,
                           r.UserName,
                           StartDate = r.StartDate.ToString("d"),
                           EndDate = r.EndDate.ToString("d"),
                           r.ClockPlan,
                           StartTime = r.StartTime.ToString("t"),
                           EndTime = r.EndTime.ToString("t"),
                           NeedClock = "正常考勤",
                           Ratio = "100",
                           r.OperatePerson,
                           r.PassWord

                       };
            if (!OperatePerson.Equals(UserAdmin))
                data = data.Where(r => r.OperatePerson.Equals(OperatePerson.Trim()));
            // 2.设置单元格抬头
            // key：实体对象属性名称，可通过反射获取值
            // value：Excel列的名称
            Dictionary<string, string> cellheader = new Dictionary<string, string> {
                    { "CardID", "工号" },
                    { "UserName", "名字" },
                    { "StartDate", "考勤起" },
                    { "EndDate", "考勤结束" },
                    { "ClockPlan", "考勤安排" },
                    { "StartTime", "签到" },
                    { "EndTime","签退"},
                    { "NeedClock","是否考勤"},
                    { "Ratio","考勤率"},
                    { "OperatePerson","下单人"},
                    { "PassWord","密码"},
                };
            string urlPath = ExcelHelper.EntityListToExcel2003(cellheader, data.ToList(), "导出信息");
            // return File(urlPath, "text/plain");
            return Json(urlPath, JsonRequestBehavior.AllowGet); //File(urlPath, "text/plain", "ceshi.xls"); //welcome.txt是客户端保存的名字
        }
        /// <summary>
        /// 存在该用户
        /// </summary>
        /// <param name="OperatePerson"></param>
        /// <returns></returns>
        public bool GetOperatePerson(string OperatePerson)
        {
            var data1 = from r in Db.UserModel.Where(r => r.UserName.Equals(OperatePerson.Trim()))
                        select r;
            return data1.Count() > 0 || OperatePerson.Equals(UserAdmin);
        }

        public ActionResult Login()
        {
            var OperatePerson = Request["OperatePerson"] ?? "";
            if (OperatePerson.Equals("") || !GetOperatePerson(OperatePerson))
            {
                return Json("请检查登入账号！");
            }
            if (OperatePerson.Equals(UserAdmin))
            {
                return Json("admin");
            }
            return Json("OK");
        }
        [HttpPost]
        public ActionResult Edit()
        {
            try
            {
                var guid = Guid.Parse(Request["guid"].ToString());
                var EditCardId = Request["EditCardId"].ToString().Trim();
                var EditEmployeeName = Request["EditEmployeeName"].ToString().Trim();
                var EditClassName = Request["EditClassName"].ToString().Trim();
                var EditStartTime = Convert.ToDateTime(Request["EditStartTime"]);
                //var editData = (from r in Db.ClockBatch
                //                where r.guid.Equals(guid)
                //                select r).FirstOrDefault();

                //editData.CardId = EditCardId;
                //editData.EmployeeName = EditEmployeeName;
                //editData.ClassName = EditClassName;
                //editData.StartClockTime = EditStartTime;
                //Db.ClockBatch.Add(editData);
                //Db.Entry<ClockBatch>(editData).State = EntityState.Modified;
                if (Db.SaveChanges() > 0)
                {
                    return Json("OK");
                }
                else
                {
                    return Json("修改失败");
                }
            }
            catch (Exception ex)
            {
                return Json($"修改失败,请检查数据格式！异常信息：{ex}");
            }

        }


        public ActionResult DeleteUser(string Id)
        {
            try
            {
                string[] ids = Id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (ids == null) return View("Index");
                foreach (var id in ids)
                {
                    var guid = Guid.Parse(id);
                    var model1 = (from r in Db.UserModel
                                  where r.ID.Equals(guid)
                                  select r).FirstOrDefault();
                    model1.Flag = false;
                    model1.LastModifyTime = DateTime.Now;
                    Db.UserModel.Add(model1);
                    Db.Entry<UserModel>(model1).State = EntityState.Modified;
                }
                Db.SaveChanges();
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(string.Format("删除失败，请检查链路是否通畅！异常信息：{0}", ex));
            }
        }
        public ActionResult Delete(string Id)
        {
            try
            {
                string[] ids = Id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (ids == null) return View("Index");
                foreach (var id in ids)
                {
                    var guid = Guid.Parse(id);
                    var model1 = (from r in Db.AgentReportModel
                                  where r.ID.Equals(guid)
                                  select r).FirstOrDefault();
                    model1.Flag = false;
                    model1.LastModifyTime = DateTime.Now;
                    Db.AgentReportModel.Add(model1);
                    Db.Entry<AgentReportModel>(model1).State = EntityState.Modified;
                }
                Db.SaveChanges();
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(string.Format("删除失败，请检查链路是否通畅！异常信息：{0}", ex));
            }
        }
    }
}