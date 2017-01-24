using IDDD.App.Cqs.QueryResult.Todos;
using IDDD.Common.Cqs.Query;
using System.Collections.Generic;

namespace IDDD.App.Cqs.Queries.Todos
{
    public class AllTodosQuery : IQuery<IEnumerable<TodoResult>> { }
}
