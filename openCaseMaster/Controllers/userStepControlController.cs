using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using openCaseMaster.ViewModels;
using Newtonsoft.Json;

namespace openCaseMaster.Controllers
{
    public class userStepControlController : Controller
    {
        // GET: userStepControl
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string myControlInit()
        {
            int userID = userHelper.getUserID();
            //string Permission =  userHelper.getUserPermission();

            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                var myControls = (from t in QC_DB.M_testCaseSteps
                                where t.userID==userID
                                orderby t.caseFramework.ID
                                select t).ToList();

                List<frameTreeNode> rootNodes = new List<frameTreeNode>();

                var frames = from t in QC_DB.caseFramework
                             where t.userID == 1
                             select t;

                foreach(var f in frames)
                {
                    frameTreeNode tmp = new frameTreeNode();
                    tmp.FID = f.ID;
                    tmp.text = f.workName;
                    tmp.iconCls = "icon-star_boxed_empty";
                    
                    rootNodes.Add(tmp);
                }

              
                foreach (var c in myControls)
                {
                    controlNode controlNode = new controlNode();
                    controlNode.id = c.ID;
                    controlNode.text = c.name;
                    controlNode.FID = c.FID.Value;

                    foreach (var r in rootNodes)
                    {
                        if(r.FID==c.FID)
                        {
                            r.children.Add(controlNode);
                            r.iconCls = "icon-star_boxed_full";
                           
                        }
                    }
                }
             


                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;

                string json = JsonConvert.SerializeObject(rootNodes, jSetting);

                return json;

            }
        
        }


        public ActionResult myControlView(int id)
        {
            controlViewModel v = new controlViewModel();
            return PartialView("_controlView", v);
        }


        public string getFrameControl(int id)
        {
            return treeHelper.getFrameControl(id);

        }
        


    }
}