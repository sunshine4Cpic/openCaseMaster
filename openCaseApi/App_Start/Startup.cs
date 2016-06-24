using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using openCaseApi.Provider;



// Owin 启动配置，不需要 Global 了
[assembly: OwinStartup(typeof(openCaseApi.Startup))]
namespace openCaseApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            /*
            var config = new HttpConfiguration();

            #region Autofac configuration (Ioc 不是必须选项)

            var builder = new ContainerBuilder();
            
            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            builder.RegisterType<ProductsRepository>().As<IProductsRepository>().InstancePerRequest();
            builder.RegisterType<RefreshTokenRepository>().As<IRefreshTokenRepository>().InstancePerRequest();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            foreach (var registration in container.ComponentRegistry.Registrations)
            {
                System.Diagnostics.Debug.WriteLine("{0}    {1}", registration.Activator, registration.Target.Services.First());
            }
            
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);

            #endregion
             */

            #region OWIN实现的OAuth

            var oAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),  // 用于决定获取token的url   http://127.0.0.1/token
                Provider = new OAuth2AuthorizationServerProvider(), // 生成和验证 access token
                //AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(10),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),//token过期时间
                AllowInsecureHttp = true, // 是否允许不安全的链接，即用不用https
                RefreshTokenProvider = new OAuth2RefreshTokenProvider(new RefreshTokenRepository())  // 生成和删除 refresh token
            };
            // 使应用程序可以使用不记名令牌来验证用户身份
            app.UseOAuthBearerTokens(oAuthOptions);
            
            #endregion

            //WebApiConfig.Register(config);
            // 这个要放在OWIN下面，OAUTH才生效
            //app.UseWebApi(config);
        }
    }
}