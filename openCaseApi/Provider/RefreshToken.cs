using System;

namespace openCaseApi.Provider
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string ClientId { get; set; }
        public DateTime IssuedUtc { get; set; }
       
        public DateTime ExpiresUtc { get; set; }
        public string ProtectedTicket { get; set; }
    }
}