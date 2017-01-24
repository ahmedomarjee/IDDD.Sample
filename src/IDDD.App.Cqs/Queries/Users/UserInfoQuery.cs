using System;
using IDDD.Common.Cqs.Query;
using IDDD.App.Cqs.QueryResult.Users;

namespace IDDD.App.Cqs.Queries.Users
{
    public class UserInfoQuery : IQuery<UserInfoResult>
    {
        public Guid Id { get; }

        public UserInfoQuery(Guid id)
        {
            Id = id;
        }
    }
}