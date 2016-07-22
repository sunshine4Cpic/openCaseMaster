using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace openCaseMaster.Models
{
    public class frameworkHelp
    {
        /// <summary>
        /// 获取默认框架
        /// </summary>
        /// <returns></returns>
        public static List<caseFramework> getAutoFramework()
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;

            List<caseFramework> cfs = objCache.Get("Framework") as List<caseFramework>;

            if (cfs == null)
            {
                QCTESTEntities QC_DB = new QCTESTEntities();

                cfs = (from t in QC_DB.caseFramework
                       where t.userID == 1
                       select t).ToList();
                //10不操作分钟过期
                //objCache.Insert("Framework", cfs, null, DateTime.MaxValue, TimeSpan.FromMinutes(10));

                //强制10分钟过期
                objCache.Insert("Framework", cfs, null, DateTime.Now.AddMinutes(10), TimeSpan.Zero);
            }

            return cfs;

        }
    }
}