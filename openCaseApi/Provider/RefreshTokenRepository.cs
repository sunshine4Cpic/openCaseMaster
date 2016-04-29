using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using Newtonsoft.Json;
using System;


namespace openCaseApi.Provider
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly string _jsonFilePath;
        private List<RefreshToken> _refreshTokens;

        public RefreshTokenRepository()
        {
            _jsonFilePath = HostingEnvironment.MapPath("~/App_Data/RefreshToken.json");
            if (File.Exists(_jsonFilePath))
            {
                var json = File.ReadAllText(_jsonFilePath);
                _refreshTokens = JsonConvert.DeserializeObject<List<RefreshToken>>(json);
            }
            if (_refreshTokens == null)
                _refreshTokens = new List<RefreshToken>();
            else
                _refreshTokens.RemoveAll(t => t.ExpiresUtc < DateTime.Now);
         
        }

        public async Task<RefreshToken> Get(string refreshToken)
        {
            return _refreshTokens.FirstOrDefault(r => r.Token == refreshToken);
        }

        public async Task<bool> Save(RefreshToken refreshToken)
        {
            _refreshTokens.Add(refreshToken);
            await WriteJsonToFile();
            return true;
        }

        public async Task<bool> Remove(string refreshToken)
        {
            _refreshTokens.RemoveAll(r => r.Token == refreshToken);
            await WriteJsonToFile();
            return true;
        }

        private async Task WriteJsonToFile()
        {
            using (var twriter = TextWriter.Synchronized(new StreamWriter(_jsonFilePath, false)))
            {
                await twriter.WriteAsync(JsonConvert.SerializeObject(_refreshTokens, Formatting.Indented));
            }
        }
    }
}