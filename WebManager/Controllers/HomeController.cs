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
                var data1 = from r in Db.AgentReportModel.ToList()
                            select new
                            {
                                r.ID,
                                r.OperatePerson,
                                r.CardID,
                                StartDate = r.StartDate.ToString("d"),
                                StartTime= r.StartTime.ToString("t"),
                                EndTime=r.EndTime.ToString("t"),
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
                string[] AddCardIds = Request["AddCardId"].ToString().Split(new char[] { '，',',' }, StringSplitOptions.RemoveEmptyEntries);
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

        /// <summary>
        /// 存在该用户
        /// </summary>
        /// <param name="OperatePerson"></param>
        /// <returns></returns>
        public bool GetOperatePerson(string OperatePerson)
        {
            var datatest = from r in Db.AgentReportModel select r;
            var data1 = from r in Db.AgentReportModel.Where(r => r.OperatePerson.Equals(OperatePerson.Trim()))
                        select new
                        {
                            r.OperatePerson,
                        };
            return data1.Count() > 0 || OperatePerson.Equals(UserAdmin);
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