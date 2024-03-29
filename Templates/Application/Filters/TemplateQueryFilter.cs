using System;
using System.Linq;
using System.Linq.Expressions;
using {{Namespace1}}.Core;
using {{Namespace2}}.Domain.Entities;

namespace {{Namespace2}}.Application.Filters
{
    /// <summary>
    /// {{ENTITY_NAME_CN}}查询过滤器
    /// </summary>
    public class {{Template}}QueryFilter : QueryFilter<{{Template}}>
    {
{{PROPERTIES}}
        /// <summary>
        /// 查询表达式
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<{{Template}}, bool>> Expression()
        {
            Expression<Func<{{Template}}, bool>> expression = s => true;

{{EXPRESSION}}
            return expression;
        }
    }
}