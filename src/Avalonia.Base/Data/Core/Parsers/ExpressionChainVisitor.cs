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

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null)
            {
                var link = Expression.Lambda(node.Expression, _rootExpression.Parameters);
                _links.Add(link.Compile());
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object != null)
            {
                var link = Expression.Lambda(node.Object, _rootExpression.Parameters);
                _links.Add(link.Compile());
            }

            return base.VisitMethodCall(node);
        }
    }
}
