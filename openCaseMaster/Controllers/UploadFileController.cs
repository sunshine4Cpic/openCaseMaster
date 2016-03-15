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
        
        [HttpPost]
        public void uploadScene(int id)
        {

            if (Request.Files.Count > 0)
            {
                Stream stm = null;
                string name = Request["name"];
                try
                {

                    for (int j = 0; j < Request.Files.Count; j++)
                    {

                        var uploadFile = Request.Files[j];
                        int offset = Convert.ToInt32(Request["chunk"]); //当前分块
                        int total = Convert.ToInt32(Request["chunks"]);//总的分块数量

                        //文件没有分块
                        if (total == 1)
                        {

                            if (uploadFile.ContentLength > 0)
                            {
                                #region 注释
                                /**** 写文件
                                if (!Directory.Exists(updir))
                                {
                                    Directory.CreateDirectory(updir);
                                }
                              //  string fileId = DateTime.Now.ToString("yyyyMMddHHmmssfff") + uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf("."));
                                uploadFile.SaveAs(string.Format("{0}\\{1}", updir, name));
                                 * ***/

                                //System.IO.Stream MyStream;
                                //int FileLen;
                                //FileLen = uploadFile.ContentLength;
                                // 读取文件的 byte[]    
                                //byte[] bytes = new byte[FileLen];
                                //MyStream = 
                                //MyStream.Read(bytes, 0, FileLen);
                                #endregion 注释
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

                                //删除文件
                                System.IO.FileInfo fi = new System.IO.FileInfo(fullname);
                                fi.Delete();

                                #region 注释
                                /*保存文件
                                if (!Directory.Exists(updir))
                                {
                                    Directory.CreateDirectory(updir);
                                }
                                string oldFullName = string.Format("{0}\\{1}", updir, name);
                                FileInfo oldFi = new FileInfo(oldFullName);
                                if (oldFi.Exists)
                                {
                                    //文件名存在则删除旧文件 
                                    oldFi.Delete();
                                }
                                fi.MoveTo(oldFullName);
                                 */
                                #endregion 注释
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("Message" + ex.ToString());
                    Response.StatusCode = 500;
                }

                try
                {
                    if (stm != null)
                    {
                        /*
                        string type = Request.QueryString["type"];
                        if (type == "case")
                        {
                            StreamReader sr = new StreamReader(stm);
                            XElement tp = XElement.Parse(sr.ReadToEnd());
                            //开始写入数据库
                            string pid = Request.QueryString["pid"];
                            string id = Request.QueryString["id"];

                            QCTESTEntities QC_DB = new QCTESTEntities();

                            M_testCase mtc = new M_testCase();
                            mtc.type = 1;
                            mtc.testXML = tp.ToString();

                            if (pid!=null)
                                mtc.projectID = Convert.ToInt32(pid);
                            if (id != null)
                                mtc.baseID = Convert.ToInt32(id);
                            //mtc.Name = name.Remove(name.LastIndexOf("."));
                            mtc.Name = tp.Attribute("desc").Value;
                            QC_DB.M_testCase.Add(mtc);
                            QC_DB.SaveChanges();
                        }
                        else if (type == "scene")
                        {
                            int id = Convert.ToInt32(Request.QueryString["id"]);
                            //场景
                            ExcelHelper.creatScene(stm, id, name.Remove(name.LastIndexOf(".")));
                        }*/
                      
                        //场景
                        ExcelHelper.creatScene(stm, id, name.Remove(name.LastIndexOf(".")));

                    }



                }
                catch (Exception ex)
                {
                    Response.Write("Message" + ex.ToString());
                    Response.StatusCode = 500;
                }
            }
        }



        /// <summary> 
        /// 从文件读取 Stream 
        /// </summary> 
        public Stream FileToStream(string fileName)
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
        private string WriteTempFile(HttpPostedFileBase uploadFile, int chunk, string name)
        {
            // string fileId = DateTime.Now.ToString("yyyyMMddHHmmssfff") + uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf("."));
            string tempDir = Server.MapPath("uploadTmp");
            

            string fullName = string.Format("{0}\\{1}.part", tempDir, name);



            if (chunk == 0)
            {
                if (System.IO.File.Exists(fullName))
                {
                    FileInfo fi = new FileInfo(fullName);
                    if ((DateTime.Now - fi.LastWriteTime).Ticks / 10000000 < 600)//600秒
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