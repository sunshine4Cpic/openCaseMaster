using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseApi.Models
{
    public class DeviceModel
    {
        public int IMEI { get; set; }

        public string system { get; set; }

        public string state { get; set; }

        public string version { get; set; }


    }
}