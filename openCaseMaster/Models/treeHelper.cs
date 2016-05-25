using Newtonsoft.Json;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

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


        /// <summary>
        /// 获取项目步骤
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="FID"></param>
        /// <returns></returns>
        public static string getProjectControl(int PID, int FID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var fps = (from t in QC_DB.Framework4Project
                       where t.PID == PID && t.FID == FID
                       select t).ToList();

            List<scriptStepTreeModel> lss = new List<scriptStepTreeModel>();

            scriptStepTreeModel rootnode = new scriptStepTreeModel();

            var p = QC_DB.project.First(t => t.ID == PID);
            rootnode.text = p.Pname;
            rootnode.PID = p.ID;
            rootnode.state = "open";

            rootnode.children = new List<treeViewModel>();
            foreach (var fp in fps)
            {
                rootnode.children.Add(fp.getControlJson4Tree());
            }
            if(rootnode.children.Count>0)
                lss.Add(rootnode);

            JsonSerializerSettings jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;

            return JsonConvert.SerializeObject(lss, jSetting);
        }




        public static string getUserControl(int FID,int PID)
        {
            int userID = userHelper.UserID;

         
            QCTESTEntities QC_DB = new QCTESTEntities();


            //我的组件
            var myControls = (from t in QC_DB.M_testCaseSteps
                              where t.userID == userID
                              select t).ToList();
            var frames = (from t in QC_DB.caseFramework
                          where t.userID == 1 && t.ID == FID
                          select t).ToList();

            var pjs = (from t in QC_DB.project
                       where t.ID == PID
                       select t).ToList();


            return getUserControl(myControls, frames, pjs);


            
        }


        public static string getUserControl()
        {
            int userID = userHelper.UserID;

            
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                //我的组件
                var myControls = (from t in QC_DB.M_testCaseSteps
                                  where t.userID == userID && t.state!=0
                                  select t).ToList();
                var frames = (from t in QC_DB.caseFramework
                             where t.userID == 1
                             select t).ToList();

                List<project> pjs = userHelper.getPermissionsProject().ToList();
                


                return getUserControl(myControls, frames, pjs);

            }
        }


        private static string getUserControl(List<M_testCaseSteps> myControls, List<caseFramework> frames, List<project> pjs)
        {


            List<frameTreeNode> frameNodes = new List<frameTreeNode>();

           
            foreach (var f in frames)
            {
                frameTreeNode tmp = new frameTreeNode();
                tmp.FID = f.ID;
                tmp.text = f.workName;

                frameNodes.Add(tmp);
            }


            //项目节点
         

            foreach (var r in frameNodes)
            {
                foreach (var p in pjs)
                {

                    projectTreeNode tmpP = new projectTreeNode();
                    tmpP.PID = p.ID;
                    tmpP.text = p.Pname;
                    



                    //control 分组
                    var tmpControls = from t in myControls
                                      where t.PID == p.ID && t.FID == r.FID
                                      select t;

                    if (tmpControls.Count() > 0)//没有control
                    {
                        r.children.Add(tmpP);
                    }

                    foreach (var c in tmpControls)
                    {
                        controlNode cn = new controlNode();
                        cn.id = c.ID;
                        //cn.text = c.name;

                        cn.name = "userstep_" + c.ID;
                        cn.desc = c.name;
                        cn.FID = c.FID.Value;
                        cn.PID = c.PID.Value;
                        tmpP.children.Add(cn);
                    }
                }

                //没有分组的control
                var noControls = from t in myControls
                                  where t.PID == null && t.FID == r.FID
                                  select t;

                foreach (var c in noControls)
                {
                    controlNode cn = new controlNode();
                    cn.id = c.ID;
                    cn.name = "userstep_" + c.ID;
                    cn.desc = c.name;
                    cn.FID = c.FID;
                    cn.PID = c.PID;
                    r.children.Add(cn);
                }

            }


            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;

            string json = JsonConvert.SerializeObject(frameNodes, jSetting);

            return json;
        }




    }
}