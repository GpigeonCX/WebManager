using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebManager.Models
{
    [Table("UserModels")]
    /// <summary>
    /// 用户注册信息
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public Guid ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPwd { get; set; }

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

        public UserModel(string name, string pwd)
        {
            this.ID = Guid.NewGuid();
            this.CreateTime = DateTime.Now;
            this.LastModifyTime = DateTime.Now;
            this.UserName = name;
            this.UserPwd = pwd;
            this.Flag = true;
        }

        public UserModel() { }
    }
}