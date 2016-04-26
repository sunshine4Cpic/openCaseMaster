using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace openCaseMaster
{
    public class Call_Client
    {
        /// <summary>
        /// DEBUG调用
        /// </summary>
        /// <param name="ClientIP">IP地址</param>
        /// <param name="msg">文件</param>
        /// <returns></returns>
        public static bool Debug(string ClientIP, string msg)
        {
            try
            {
                //创建连接
                HttpWebRequest mHttpRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + ClientIP + ":8500/testM/?runType=Debug");
                //超时间毫秒为单位
                mHttpRequest.Timeout = 180000;
                //发送请求的方式
                mHttpRequest.Method = "POST";
                //发送的协议
                mHttpRequest.Accept = "HTTP";
                //字节范围
                mHttpRequest.AddRange(100, 10000);
                //是否和请求一起发送
                mHttpRequest.UseDefaultCredentials = true;
                //写数据信息的流对象
                //C:\Users\Admin\Desktop\test2.xml
                StreamWriter swMessages = new StreamWriter(mHttpRequest.GetRequestStream());
                //写入的流以XMl格式写入
                swMessages.Write(msg);
                //关闭写入流
                swMessages.Close();
                //创建一个响应对象
                HttpWebResponse mHttpResponse = (HttpWebResponse)mHttpRequest.GetResponse();
                if (mHttpResponse.StatusDescription == "OK")
                {

                }
                mHttpResponse.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }



        public static bool startRun(string url)
        {
            try
            {
                //创建连接
                HttpWebRequest mHttpRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + url + "/testM/?runType=Scene");
                //超时间毫秒为单位
                mHttpRequest.Timeout = 60000;
                //发送请求的方式
                mHttpRequest.Method = "GET";

                //字节范围
                //mHttpRequest.AddRange(100, 10000);
                //是否和请求一起发送
                //mHttpRequest.UseDefaultCredentials = true;
                //写数据信息的流对象
                //C:\Users\Admin\Desktop\test2.xml
                //StreamWriter swMessages = new StreamWriter(mHttpRequest.GetRequestStream());
                //写入的流以XMl格式写入
                //swMessages.Write(msg);
                //关闭写入流
                //swMessages.Close();
                //创建一个响应对象
                HttpWebResponse mHttpResponse = (HttpWebResponse)mHttpRequest.GetResponse();
                if (mHttpResponse.StatusDescription == "OK")
                {
                    mHttpResponse.Close();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }


        }

        public static bool Package(string ClientIP, string packagemsg)
        {
            try
            {

                //创建连接,httpweb请求
                HttpWebRequest PackageHttpRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + ClientIP + ":8500/testM/?runType=Package");
                //超时间毫秒为单位
                PackageHttpRequest.Timeout = 180000;
                //发送请求的方式
                PackageHttpRequest.Method = "POST";
                //请求类型
                PackageHttpRequest.ContentType = "application/octet-stream";
                //发送的协议
                PackageHttpRequest.Accept = "HTTP";
                //字节范围
                PackageHttpRequest.AddRange(100, 10000);
                //是否和请求一起发送
                PackageHttpRequest.UseDefaultCredentials = true;
                //写数据信息的流对象

                StreamWriter PackageMessages = new StreamWriter(PackageHttpRequest.GetRequestStream());

                PackageMessages.Write(packagemsg);

                //关闭写入流
                PackageMessages.Close();
                //创建一个响应对象
                HttpWebResponse mHttpResponse = (HttpWebResponse)PackageHttpRequest.GetResponse();

                if (mHttpResponse.StatusDescription == "OK")
                {

                }
                mHttpResponse.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }



        }



    }
}