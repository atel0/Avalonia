using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Avalonia.Data.Core.Parsers
{
    internal class ExpressionChainVisitor : ExpressionVisitor
    {
        private readonly LambdaExpression _rootExpression;
        private List<Delegate> _links = new List<Delegate>();

        public ExpressionChainVisitor(LambdaExpression expression)
        {
            _rootExpression = expression;
        }

        public static List<Delegate> Build<TIn, TOut>(Expression<Func<TIn, TOut>> expression)
        {
            var visitor = new ExpressionChainVisitor(expression);
            visitor.Visit(expression);
            visitor._links.Reverse();
            return visitor._links;
        }

        public override Expression Visit(Expression node)
        {
            if (node != null && node != _rootExpression)
            {
                var link = Expression.Lambda(node, _rootExpression.Parameters);
                _links.Add(link.Compile());
            }

            return base.Visit(node);
        }
    }
}
