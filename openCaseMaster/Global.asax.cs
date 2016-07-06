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


                        JObject userJ = JObject.Parse(userData);

                        string[] roles = userJ["Roles"].ToString().Split(',');

                        int ID = Convert.ToInt32(userJ["ID"]);

                        string userName = userJ["userName"].ToString();

                        string Permission = userJ["Permission"].ToString();


                        //重建HttpContext.Current.User，加入用户拥有的角色数组 
                        HttpContext.Current.User = new FormUser(id, roles, ID, userName, Permission);


                    }
                }
            }

        } 



    }


    public class FormUser:GenericPrincipal
    {
        public FormUser(IIdentity identity, string[] roles, int ID, string userName, string Permission)
            : base(identity, roles)
        {
            this.ID = ID;
            this.userName = userName;
            this.Permission = Permission;
        }

        public int ID { get; set; }
        public string userName { get; set; }
        public string Permission { get; set; }
    }



}
