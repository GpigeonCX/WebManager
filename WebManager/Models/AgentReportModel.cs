using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebManager.Models
{
    /// <summary>
    /// 代理报单实体
    /// </summary>
    public class AgentReportModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public Guid ID { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string CardID { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 考勤起
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 考勤结束
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 考勤计划 1234567
        /// </summary>
        public int ClockPlan { get; set; }

        /// <summary>
        /// 打卡开始时间 7:00
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 打卡结束时间 17:30
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否考勤
        /// </summary>
        public bool NeedClock { get; set; }

        /// <summary>
        /// 考勤率
        /// </summary>
        public int Ratio { get; set; }

        /// <summary>
        /// 下单人
        /// </summary>
        public string OperatePerson { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后操作时间
        /// </summary>
        public DateTime LastModifyTime { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Flag { get; set; }

        public AgentReportModel() { }
    }
}