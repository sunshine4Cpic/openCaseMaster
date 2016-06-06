using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using openCaseMaster.SignalR;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartupAttribute(typeof(openCaseMaster.Startup))]

namespace openCaseMaster
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var userIdProvider = new MyUserFactory();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => userIdProvider);

            app.MapSignalR();
        }
    }
}
