using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace openCaseMaster.ViewModels
{
    [DataContract]
    public class testDemandTree :treeViewModel
    {
        public testDemandTree(M_testDemand td)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            DemandID = td.ID;
            text = td.name;
            state = td.type == 0 ? "closed" : "open";
            iconCls = td.type == 0 ? null : "icon-application_windows";
            type = td.type == null ? 0 : td.type.Value;

            if (td.type != 1) return;

            if (td.isRun == true)//需优化
            {
                var TotalCase = new ObjectParameter("Out1", DbType.Int32);
                var sucess = new ObjectParameter("Out2", DbType.Int32);
                var error = new ObjectParameter("Out3", DbType.Int32);
                var noRun = new ObjectParameter("Out4", DbType.Int32);
                var Untreated = new ObjectParameter("Out5", DbType.Int32);

                QC_DB.M_testDemand_result(td.ID, TotalCase, sucess, error, noRun, Untreated);

                if (Convert.ToInt32(noRun.Value) == 0)
                {
                    iconCls = "icon-application_windows_okay";
                    if (Convert.ToInt32(Untreated.Value) > 0)//未处理>0
                        iconCls = "icon-application_windows_edit";
                }
                else if (Convert.ToInt32(noRun.Value) > 0)
                    iconCls = "icon-application_windows_right";
            }

        }
        [DataMember]
        public int DemandID { get; set; }

        [DataMember]
        public int type { get; set; }
    }

    [DataContract]
    public class testSceneTree : treeViewModel
    {
        [DataMember]
        public int? DemandID { get; set; }

        [DataMember]
        public int type { get; set; }
    }
}