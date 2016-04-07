using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.ViewModels
{
    public class registerDevice_req
    {
        /// <summary>
        /// 设备device
        /// </summary>
        [Required(ErrorMessage="缺少device")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "device必须为4~50个字符")]
        public string device { get; set; }


        /// <summary>
        /// 执行机IP
        /// </summary>
        [Required(ErrorMessage = "缺少IP")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "IP必须为4~50个字符")]
        public string IP { get; set; }

        /// <summary>
        /// 型号(描述)
        /// </summary>
        [StringLength(50, ErrorMessage = "model不能超过50个字符")]
        public string model { get; set; }

    }

    public class caseResult_req
    {

        /// <summary>
        /// 结果文件
        /// </summary>
        [Required]
        public XElement resultXML { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        public DateTime startDate { get; set; }


        /// <summary>
        /// 结束时间
        /// </summary>
        [Required]
        public DateTime endDate { get; set; }

        /// <summary>
        /// 状态 1完成 null未执行
        /// </summary>
        [Required]
        public int? state { get; set; }


        /// <summary>
        /// 结果目录
        /// </summary>
        [Required]
        public string resultPath { get; set; }


       

    }


    public class application_res
    {

        public application_res(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            //属于这台手机并已经开始的场景
            var app = QC_DB.M_application.First(t => t.ID == id);

            this.id = app.ID;

            this.id = app.ID;
            this.name = app.name;
            this.androidPackeg = app.package;
            this.mainActivity = app.mainActiviy;
            this.iosPackeg = app.package2;
            this.clearCache = app.isClear==true?true:false;

        }

      
        public int id { get; set; }

        /// <summary>
        /// app名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// android包名
        /// </summary>
        public string androidPackeg { get; set; }

        /// <summary>
        /// android启动Activity
        /// </summary>
        public string mainActivity{ get; set; }


        /// <summary>
        /// ios包名
        /// </summary>
        public string iosPackeg { get; set; }

        /// <summary>
        /// 清楚缓存
        /// </summary>
        public bool clearCache { get; set; }


    }

    
}