using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Modding.Core
{
    public class DeepClone<T>
    {
        private static readonly Dictionary<Type, Func<T, T>> _cacheExpressionTree = new Dictionary<Type, Func<T, T>>();
        public static T ExpressionCopy(T original)
        {
            var type = typeof(T);
            if (!_cacheExpressionTree.TryGetValue(type, out var func))
            {
                var originalParam = Expression.Parameter(type, "original");
                var clone = Expression.Variable(type, "clone");
                var expressions = new List<Expression>();
                expressions.Add(Expression.Assign(clone, Expression.New(type)));
                foreach (var prop in type.GetProperties())
                {
                    var originalProp = Expression.Property(originalParam, prop);
                    var cloneProp = Expression.Property(clone, prop);
                    expressions.Add(Expression.Assign(cloneProp, originalProp));
                }
                expressions.Add(clone);
                var lambda = Expression.Lambda<Func<T, T>>(Expression.Block(new[] { clone }, expressions), originalParam);
                func = lambda.Compile();
                _cacheExpressionTree.Add(type, func);
            }
            return func(original);
        }
    }
}
