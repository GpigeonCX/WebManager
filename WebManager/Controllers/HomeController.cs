using System;
using System.Collections.Generic;
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

        public ActionResult GetEmployee()

        {
            try
            {
                int pageIndex = Request["page"] == null ? 1 : int.Parse(Request["page"]);
                DBContext Db1 = new DBContext();
                int pageSize = Request["limit"] == null ? 50 : int.Parse(Request["limit"]);
                var OperatePerson = Request["OperatePerson"].ToString();
                if (OperatePerson.Equals(""))
                    return Json(null);
                var data1 = from r in Db.AgentReportModel
                            select new
                            {
                                r.OperatePerson,
                                r.CardID,
                                StartDate=r.StartDate.ToString("t"),
                            };
                data1 = data1.Where(r => r.OperatePerson.Equals(OperatePerson.Trim()));
                int total = data1.Count();//总条数
                                          //构造成Json的格式传递
                var result = new { code = 0, msg = "123", count = total, data = data1.ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize) };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null);
            }


        }
        [HttpPost]
        public ActionResult Add()
        {
            try
            {
                var StartTime = Convert.ToDateTime(Request["AddStartTime"]);
                var ClassName = Request["AddClassName"].ToString().Trim();
                //string[] CardId = Request["CardId"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] AddInfo = Request["AddInfo"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] info;
                if (AddInfo.Length > 0)
                {
                    foreach (var item in AddInfo)
                    {
                        info = item.Split('-');
                        ClockBatch model = new ClockBatch
                        {
                            guid = Guid.NewGuid(),
                            EmployeeName = info[1],
                            CardId = info[0].Trim(),
                            ClassName = ClassName,
                            StartClockTime = StartTime,
                            LastClockTime = Convert.ToDateTime("2000-1-1 00:00"),
                            ClockState = false,
                            FailedReason = "新增的数据，还没有打卡记录！" + DateTime.Now.ToString(),
                            flag = true
                        };
                        Db.ClockBatch.Add(model);
                        Db.Entry<ClockBatch>(model).State = EntityState.Added;
                    }
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
                return Json($"添加失败,请检查数据格式,使用英文符号！异常信息：{ex}");
            }

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
                    //var model1 = new ClockBatch() { guid = Guid.Parse(id) };
                    //Db.ClockBatch.Attach(model1);
                    //Db.Entry<ClockBatch>(model1).State = EntityState.Deleted;
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