using IDDD.Core.Domain;
using IDDD.Domain.Membership.Clients;
using System;
using System.Collections.Generic;

namespace IDDD.Domain.Membership.Tokens
{
    public class RefreshToken: Entity
    {
        public RefreshTokenId Id { get; set; }

        public ClientId ClientId { get; set; }
        public IdentityUserId UserId { get; set; }
        public string IpAddress { get; set; }

        public DateTime? ExpiresUtc { get; set; }
        public DateTime? IssuedUtc { get; set; }

        public string TicketId { get; set; }

        protected override IEnumerable<object> GetIdentityComponents()
        {
            yield return this.ClientId;
            yield return this.ExpiresUtc;
            yield return this.Id;
            yield return this.IpAddress;
            yield return this.IssuedUtc;
            yield return this.TicketId;
            yield return this.UserId;
        }
    }
}