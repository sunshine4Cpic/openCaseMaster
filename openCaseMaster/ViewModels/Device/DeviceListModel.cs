using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class DeviceListModel
    {
        public DeviceListModel(int page, int rows)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var us = from t in QC_DB.M_deviceConfig
                     orderby t.ID
                     select new DeviceModel
                     {
                         id = t.ID,
                         mark= t.mark,
                         Model = t.Model,
                         IP = t.IP,
                         device = t.device
                     };


            this.total = us.Count();

            this.rows = us.Skip(rows * (page - 1)).Take(rows).ToList();
        }

        public int total { get; set; }

        public List<DeviceModel> rows { get; set; }
    }

    public class DeviceModel
    {
        public int id { get; set; }
        public string mark { get; set; }

        public string Model { get; set; }

        public string IP { get; set; }

        public string device { get; set; }
    }
}