using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.Infrastructure;


namespace openCaseApi.Provider
{
    public class OAuth2RefreshTokenProvider:AuthenticationTokenProvider
    {
        // refresh token 存储源可以换成 DB / Cache 等
        //private static ConcurrentDictionary<string, string> _refreshTokens = new ConcurrentDictionary<string, string>();
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public OAuth2RefreshTokenProvider(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }


        /// <summary>
        /// 生成 refresh token，并保存
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            #region Check

            // 客户端认证没有用户登录，所以不需要 RefreshToken
            if (string.IsNullOrEmpty(context.Ticket.Identity.Name)) return;


            var clientId = context.OwinContext.Get<string>("client_id");
            if (string.IsNullOrWhiteSpace(clientId)) return;

            //var refreshTokenLifeTime = context.OwinContext.Get<string>("clientRefreshTokenLifeTime"); //过期时间
            //if (string.IsNullOrWhiteSpace(refreshTokenLifeTime)) return;

            var refreshTokenLifeTime = "30";

            #endregion

            // 生成 refresh token，可以换成其他生成方式和规则
            string tokenValue = Guid.NewGuid().ToString("N");

            // 保存到DB/Cache等 
            var refreshToken = new RefreshToken
            {
                Token = tokenValue,
                ClientId = clientId,
                UserName = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddDays(Convert.ToDouble(refreshTokenLifeTime)),
                ProtectedTicket = context.SerializeTicket()
            };

            context.Ticket.Properties.IssuedUtc = refreshToken.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = refreshToken.ExpiresUtc;
            
            //_refreshTokens[tokenValue] = context.SerializeTicket();
            if(await _refreshTokenRepository.Save(refreshToken))
                context.SetToken(tokenValue);
        }

        /// <summary>
        /// 接收，移除当前 refresh token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            //string protectedToken;
            //if(_refreshTokens.TryRemove(context.Token, out protectedToken))
            //    context.DeserializeTicket(protectedToken);

            var refreshToken = await _refreshTokenRepository.Get(context.Token);

            if (refreshToken != null)
            {
                // 不能过期
                //if (refreshToken.ExpiresUtc > DateTime.UtcNow)
                context.DeserializeTicket(refreshToken.ProtectedTicket);

                var result = await _refreshTokenRepository.Remove(context.Token);
            }
        }
    }
}