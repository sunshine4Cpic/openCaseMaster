
using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace openCaseMaster.Controllers
{
    [Authorize(Roles = "user")]
    public class UploadFileController : Controller
    {
        /// <summary>
        /// 上传场景
        /// </summary>
        [HttpPost]
        public void uploadScene(int id)
        {
            
            var stm = FileToStream();
            if (stm != null)
            {
                string originalName = Request["originalName"];
                ExcelHelper.creatScene(stm, id, originalName.Remove(originalName.LastIndexOf(".")));

            }


        }

        /// <summary>
        /// 上传public脚本
        /// </summary>
        [HttpPost]
        public JsonResult uploadTaskScript()
        {

            var stm = FileToStream();
            if (stm != null)
            {
                string originalName = Request["originalName"];
                var result = from t in ExcelHelper.tmpTaskScript(stm)
                             select new
                             {
                                 ID = t.ID,
                                 title = t.title
                             };
                return Json(result); 

            }
            return null;

        }

        [HttpPost]
        public void uploadCase(int? baseID, int? PID)
        {

            var stm = FileToStream();
            if (stm != null)
            {
                StreamReader sr = new StreamReader(stm);
                XElement tp = XElement.Parse(sr.ReadToEnd());
                //开始写入数据库


                QCTESTEntities QC_DB = new QCTESTEntities();

                M_testCase mtc = new M_testCase();
                mtc.type = 1;
                mtc.testXML = tp.ToString();
                if (PID == null)
                    mtc.baseID = Convert.ToInt32(baseID);
                else
                    mtc.projectID = Convert.ToInt32(PID);
       
                //mtc.Name = name.Remove(name.LastIndexOf("."));
                mtc.Name = tp.Attribute("desc").Value;
                mtc.FID = Convert.ToInt32(tp.Attribute("FID").Value);
                QC_DB.M_testCase.Add(mtc);
                QC_DB.SaveChanges();

            }


        }

        /// <summary>
        /// 上传apk
        /// </summary>
        /// <param name="id">测试任务ID</param>
        [HttpPost]
        public string uploadApk(int id)
        {
            string originalName = "";
            var stm = FileToStream();

            if (stm != null)
            {
                string guid = System.Guid.NewGuid().ToString("N");

                originalName = guid+"_"+Request["originalName"];
                string filename = Server.MapPath("~/apkInstall/") + originalName;

                StreamWriter sw = new StreamWriter(filename);
                stm.CopyTo(sw.BaseStream);
                sw.Flush();
                sw.Close();


                QCTESTEntities QC_DB = new QCTESTEntities();

                var Mtd = QC_DB.M_testDemand.Where(t => t.ID == id).First();
                Mtd.apkName = originalName;
                QC_DB.SaveChanges();
            }

            return originalName;
        }

        [HttpPost]
        public string userFramework()
        {
            /*******直接保存不可取 提取需要的信息再保存********/
            var stm = FileToStream();
            if (stm != null)
            {
                StreamReader sr = new StreamReader(stm);
                XElement fm = XElement.Parse(sr.ReadToEnd());

                int userID = userHelper.getUserID();

                QCTESTEntities QC_DB = new QCTESTEntities();

                var cf = QC_DB.caseFramework.First(t => t.userID == userID);

                cf.controlXML = fm.ToString();
                QC_DB.SaveChanges();

                return cf.controlXML;

            }else
            {
                Response.Status = "500";
                return "文件上传失败";
            }
           
        }



        [NonAction]
        private Stream FileToStream()
        {
            if (Request.Files.Count <= 0) return null;

            Stream stm = null;
            string name = Request["name"];
            for (int j = 0; j < Request.Files.Count; j++)
            {

                var uploadFile = Request.Files[j];
                int offset = Convert.ToInt32(Request["chunk"]); //当前分块
                int total = Convert.ToInt32(Request["chunks"]);//总的分块数量

                //文件没有分块
                if (total <= 1)
                {

                    if (uploadFile.ContentLength > 0)
                    {
                        stm = uploadFile.InputStream;
                    }
                }
                else
                {

                    //文件 分成多块上传
                    string fullname = WriteTempFile(uploadFile, offset, name);
                    if (total - offset == 1)
                    {
                        //如果是最后一个分块文件 ，则把文件从临时文件夹中移到上传文件 夹中

                        stm = FileToStream(fullname);

                        //删除文件,此处可合并
                        System.IO.FileInfo fi = new System.IO.FileInfo(fullname);
                        fi.Delete();
                    }

                }
            }
            
            return stm;

        }

        /// <summary>
        /// 返回上传成功的文件流
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
         [NonAction]
        private Stream FileToStream(string fileName)
        {
            // 打开文件 
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream 
            Stream stream = new MemoryStream(bytes);
            return stream;
        }



        
        /// <summary>
        /// 保存临时文件 
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <param name="chunk"></param>
        /// <returns></returns>
         [NonAction]
        private string WriteTempFile(HttpPostedFileBase uploadFile, int chunk, string name)
        {
            // string fileId = DateTime.Now.ToString("yyyyMMddHHmmssfff") + uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf("."));
            string tempDir = Server.MapPath("/uploadTmp");
            

            string fullName = string.Format("{0}\\{1}.part", tempDir, name);



            if (chunk == 0)
            {
                if (System.IO.File.Exists(fullName))
                {
                    FileInfo fi = new FileInfo(fullName);
                    if ((DateTime.Now - fi.LastWriteTime).Ticks / 10000000 < 300)//300秒
                    {
                        throw new Exception("已经存在缓存文件");
                    }
                }
                //如果是第一个分块，则直接保存
                uploadFile.SaveAs(fullName);
            }
            else
            {
                //如果是其他分块文件 ，则原来的分块文件，读取流，然后文件最后写入相应的字节
                FileStream fs = new FileStream(fullName, FileMode.Append);
                if (uploadFile.ContentLength > 0)
                {
                    int FileLen = uploadFile.ContentLength;
                    byte[] input = new byte[FileLen];

                    // Initialize the stream.
                    System.IO.Stream MyStream = uploadFile.InputStream;

                    // Read the file into the byte array.
                    MyStream.Read(input, 0, FileLen);

                    fs.Write(input, 0, FileLen);
                    fs.Close();
                }
            }
            return fullName;
        }
    }
}