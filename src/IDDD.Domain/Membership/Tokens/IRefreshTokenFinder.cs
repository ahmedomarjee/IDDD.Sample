﻿using IDDD.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.Domain.Membership.Tokens
{
    public interface IRefreshTokenFinder: IFinder<RefreshToken, RefreshTokenId>
    {
    }
}
