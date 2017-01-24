using IDDD.Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.Domain.Membership.Tokens
{
    public interface IRefreshTokenRepository: IRepository<RefreshToken, RefreshTokenId>
    {
    }
}
