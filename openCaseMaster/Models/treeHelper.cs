using Newtonsoft.Json;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.Models
{
    public class treeHelper
    {
        
        
        /// <summary>
        /// 获取框架步骤
        /// </summary>
        /// <param name="FID"></param>
        /// <returns></returns>
        public static string getFrameControl(int? FID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            List<caseFramework> ss = new List<caseFramework>();

            if (FID == null)//没有指定框架就是全框架
            {
                ss = (from t in QC_DB.caseFramework
                      where t.userID == 1
                      select t).ToList();
            }
            else
            {
                ss = (from t in QC_DB.caseFramework
                      where t.ID == FID
                      select t).ToList();
            }


            List<treeViewModel> Fdata = new List<treeViewModel>();
            foreach (var s in ss)
            {
                var treeNode = s.getControlJson4Tree();
                Fdata.Add(treeNode);
            }

            JsonSerializerSettings jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;

            return JsonConvert.SerializeObject(Fdata, jSetting);
        }


        public static string getProjectControl(int PID,int? FID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var fps = from t in QC_DB.Framework4Project
                      where t.PID == PID
                      select t;
            if (FID != null)
                fps = fps.Where(t => t.FID == FID);

            List<treeViewModel> Pdata = new List<treeViewModel>();
            foreach (var s in fps)
            {
                var treeNode = s.getControlJson4Tree();
                Pdata.Add(treeNode);
            }
            JsonSerializerSettings jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;
           
            return JsonConvert.SerializeObject(Pdata, jSetting);
        }


        public static string getUserControl(int? FID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            int userID = userHelper.getUserID();

            var Ups = from t in QC_DB.M_testCaseSteps
                      where t.userID == userID
                      select t;
            if (FID != null)
                Ups = Ups.Where(t => t.FID == FID);

            var usts = from t in Ups
                       select new scriptStepTreeModel
                       {
                           text = t.name,
                           id = t.ID,
                           name = "userstep_" + t.ID,
                           desc = t.name,
                           iconCls = "icon-view_outline_detail"
                       };
            JsonSerializerSettings jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(usts, jSetting);
        }
    }
}