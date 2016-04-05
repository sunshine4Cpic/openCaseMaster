using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class DeviceViewModel
    {
        public int ID { get; set; }
        public string mark { get; set; }

        public string img { get; set; }

        /// <summary>
        /// 已执行
        /// </summary>
        public int run { get; set; }

        /// <summary>
        /// 执行队列
        /// </summary>
        public int runing { get; set; }

        public string Model { get; set; }
    }
}