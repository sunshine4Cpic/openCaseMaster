using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using openCaseApi.Models;

namespace openCaseApi.Provider
{
    public class OAuth2AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// 验证客户端id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            context.TryGetBasicCredentials(out clientId, out clientSecret);
            
            // clientId 和 clientSecret 也应该持久化到其他地方
            if (!(clientId == "console" && clientSecret == "test")) { 
                context.SetError("invalid_client", "客户端id或密钥不正确");
                //context.Rejected();  用了这个，上面的错误不会显示，将显示的 "error":"invalid_client"
                return;
            }

            // 这里保存的数据用于创建 refresh token 所需要的关联信息
            context.OwinContext.Set<string>("as:client_id", clientId);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", "30");   // refresh token 过期时间间隔，天

            context.Validated(clientId);

            await base.ValidateClientAuthentication(context);
        }

        /// <summary>
        /// Password认证方式实现，并发放token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var username = context.UserName;
            var password = context.Password;


            QCTESTEntities qc = new QCTESTEntities();
            var loginUser = qc.admin_user.Where
                (t => t.Username == username && t.Password == password).FirstOrDefault();
           

            // 调用自己的验证逻辑
            if (loginUser == null)
            {
                context.SetError("invalid_grant", "用户名或密码不正确。");
                return;
            }



            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));

            if(loginUser.Type==1)
                oAuthIdentity.AddClaim(new Claim(ClaimTypes.Role, "admin"));//管理员权限

            //自定义参数
            //oAuthIdentity.AddClaim(new Claim("hahaha", "测试一下"));     

            // 将 clientId 保存在 ticket 中，好在 refresh token 中验证 clientId
            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                {"as:client_id", context.ClientId}
            });
            var ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);

            await base.GrantResourceOwnerCredentials(context);
        }

        /// <summary>
        /// 客户端认证方式实现，并发放token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            // 将 clientId 保存在 ticket 中，好在 refresh token 中验证 clientId
            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                {"as:client_id", context.ClientId}
            });
            var ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
        }

        /// <summary>
        /// 验证持有 refresh token 的 clientId
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClientId = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClientId = context.ClientId;

            if (originalClientId != currentClientId)
                await Task.FromResult<object>(null);

            var newId = new ClaimsIdentity(context.Ticket.Identity);
            newId.AddClaim(new Claim("newClaim", "refreshToken"));
            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);
        }

        public override async Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            await base.ValidateTokenRequest(context);
        }
    }
}