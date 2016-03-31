using Newtonsoft.Json.Linq;
using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace openCaseMaster
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //先用Application保存,以后用缓存代替
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                List<caseFramework> ss = (from t in QC_DB.caseFramework
                                         where t.userID == 1
                                         select t).ToList();
                Application["Framework"] = ss;
            }

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //改造原来的User，给其添加一个用户所属的角色数据 
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

           
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;//cookie
                        
                        string userData = ticket.UserData;


                        try
                        {
                            JObject userJ = JObject.Parse(userData);

                            string[] roles = userJ["Roles"].ToString().Split(',');
                            //重建HttpContext.Current.User，加入用户拥有的角色数组 
                            HttpContext.Current.User = new GenericPrincipal(id, roles);
                          
                        }
                        catch 
                        {
                            //处理个毛线,以后重写,使用 MVC自带的权限控制
                          
                        } 
                      
                        
                    }
                }
            }

        } 



    }
}
