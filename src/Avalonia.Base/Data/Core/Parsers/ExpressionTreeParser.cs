using System;
using System.Linq;
using System.Linq.Expressions;

namespace Avalonia.Data.Core.Parsers
{
    static class ExpressionTreeParser
    {
        public static IExpressionNode<TIn> Parse<TIn, TOut>(Expression<Func<TIn, TOut>> expr, bool enableDataValidation)
        {
            var visitor = new ExpressionVisitorNodeBuilder(enableDataValidation);

            visitor.Visit(expr);

            var nodes = visitor.Nodes;

            ////for (int n = 0; n < nodes.Count - 1; ++n)
            ////{
            ////    nodes[n].Next = nodes[n + 1];
            ////}

            return (IExpressionNode<TIn>)nodes.FirstOrDefault() ?? new EmptyExpressionNode<TIn>();
        }
    }
}
