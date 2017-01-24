using System;

namespace IDDD.App.Cqs.QueryResult.RefreshTokens
{
    public class RefreshTokenResult
    {
        public Guid Id { get; set; }


        public string UserId { get; set; }
        public string ClientId { get; set; }

        public string IpAddress { get; set; }

        public DateTime? IssuedUtc { get; set; }
        public DateTime? ExpiresUtc { get; set; }
    }
}