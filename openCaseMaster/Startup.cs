using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(openCaseMaster.Startup))]

namespace openCaseMaster
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           

            //用户权限相关
            ConfigureAuth(app);

           
        }
    }
}
