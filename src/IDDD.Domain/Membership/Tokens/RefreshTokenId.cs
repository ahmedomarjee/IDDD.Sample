using IDDD.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.Domain.Membership.Tokens
{
    public class RefreshTokenId: Identity
    {
            public RefreshTokenId() { }

            public RefreshTokenId(string id)
                : base(id)
            {
            }
        
    }
}
