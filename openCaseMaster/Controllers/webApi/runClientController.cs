using Newtonsoft.Json;
using openCaseMaster.Models;
using openCaseMaster.ViewModels;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;


namespace openCaseMaster.Controllers
{
    
  /// <summary>
  /// 客户端调用
  /// </summary>
    public class runClientController : ApiController
    {
        /// <summary>
        /// 注册执行设备
        /// </summary>
        /// <param name="req">设备信息</param>
        /// <returns>设备备注</returns>
        [HttpPost]
        public IHttpActionResult registerDevice([FromBody]registerDevice_req req)
        {
            

             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }
            QCTESTEntities QC_DB = new QCTESTEntities();

            M_deviceConfig mdc = QC_DB.M_deviceConfig.Where(t => t.device == req.device).FirstOrDefault();
            if (mdc != null)
            {
                mdc.IP = req.IP;
            }
            else
            {
                mdc = new M_deviceConfig();
                mdc.IP = req.IP;
                mdc.device = req.device;
                mdc.Port = 8500;
                mdc.Model = req.model;
                mdc.mark = "新设备:" + req.model;
                QC_DB.M_deviceConfig.Add(mdc);
            }

            QC_DB.SaveChanges();

            //string json = JsonConvert.SerializeObject(new { mdc.mark });

            
            return Ok(mdc.mark);
            
        }



        
        /// <summary>
        /// 获取执行场景
        /// </summary>
        /// <param name="device">设备号</param>
        /// <returns></returns>
        [HttpGet]
        public AutoRunSceneModel AutoRunScene(string device)
        {
           
            AutoRunSceneModel rsm = new AutoRunSceneModel(device);
            if (rsm.id == 0)
                return null;
            return rsm; 
        }


        /// <summary>
        /// 获取执行案例
        /// </summary>
        /// <param name="id">案例ID</param>
        /// <returns>案例文件</returns>
       
        [HttpGet]
        public HttpResponseMessage RunScript(int id)
        {
           


            QCTESTEntities QC_DB = new QCTESTEntities();

           
            var rtc = QC_DB.M_runTestCase.First(t => t.ID == id);

            if (rtc.M_runScene.M_testDemand.isRun != true)
            {
                HttpResponseMessage responseMessage =
                   new HttpResponseMessage { Content = new StringContent("null", Encoding.GetEncoding("UTF-8")) };

                return responseMessage;
            }
            else
            {

                HttpResponseMessage responseMessage =
                   new HttpResponseMessage { Content = new StringContent(rtc.testXML, Encoding.GetEncoding("UTF-8"), "text/xml") };

                return responseMessage;
            }
        }



       /// <summary>
        /// 执行结果上传
       /// </summary>
       /// <param name="id">案例ID</param>
       /// <param name="req">结果集</param>
       /// <returns></returns>
        [HttpPost]
        public IHttpActionResult caseResult([FromBody]caseResult_req req)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            QCTESTEntities QC_DB = new QCTESTEntities();

            //属于这台手机并已经开始的场景
            var rtc = QC_DB.M_runTestCase.First(t => t.ID == req.ID);

            rtc.startDate = req.startDate;
            rtc.endDate = req.endDate;

            rtc.state = req.state;

            rtc.resultXML = req.resultXML.ToString();

            rtc.resultPath = req.resultPath;

            QC_DB.SaveChanges();

            return Ok();
        }
        /// <summary>
        /// 安装结果上传
        /// </summary>
        /// <param name="req">安装结果</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SceneInstallResult([FromBody]SceneInstallResult_req req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            QCTESTEntities QC_DB = new QCTESTEntities();

            //属于这台手机并已经开始的场景
            var rtc = QC_DB.M_runScene.First(t => t.ID == req.ID);

            rtc.installResult = req.installResult;

            QC_DB.SaveChanges();

            return Ok();
        }


        /// <summary>
        /// 获取app信息
        /// </summary>
        /// 
        /// <param name="id">appID</param>
        /// <returns></returns>
        [HttpGet]
        public application_res application(int? id)
        {
            application_res res = new application_res(id);
            return res;
        }

    }
}
