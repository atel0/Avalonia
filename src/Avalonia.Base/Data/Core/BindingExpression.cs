using System;
using System.Linq.Expressions;
using Avalonia.Data.Core.Parsers;

#nullable enable

namespace Avalonia.Data.Core
{
    public static class BindingExpression
    {
        public static BindingExpression<TIn, TOut> Create<TIn, TOut>(
            TIn root,
            Expression<Func<TIn, TOut>> expression)
                where TIn : class
        {
            return Create(new Single<TIn>(root), expression);
        }

        public static BindingExpression<TIn, TConverted> Create<TIn, TOut, TConverted>(
            TIn root,
            Expression<Func<TIn, TOut>> expression,
            Func<TOut, TConverted> convert)
                where TIn : class
        {
            return Create(new Single<TIn>(root), expression, convert);
        }

        public static BindingExpression<TIn, TOut> Create<TIn, TOut>(
            IObservable<TIn> root,
            Expression<Func<TIn, TOut>> expression)
                where TIn : class
        {
            return new BindingExpression<TIn, TOut>(
                root,
                expression.Compile(),
                ExpressionChainVisitor.Build(expression));
        }

        public static BindingExpression<TIn, TConverted> Create<TIn, TOut, TConverted>(
            IObservable<TIn> root,
            Expression<Func<TIn, TOut>> expression,
            Func<TOut, TConverted> convert)
                where TIn : class
        {
            var compiled = expression.Compile();

            return new BindingExpression<TIn, TConverted>(
                root,
                x => convert(compiled(x)),
                ExpressionChainVisitor.Build(expression));
        }

        private class Single<T> : IObservable<T>, IDisposable where T : class
        {
            private WeakReference<T> _value;

            public Single(T value) => _value = new WeakReference<T>(value);

            public IDisposable Subscribe(IObserver<T> observer)
            {
                if (_value.TryGetTarget(out var value))
                {
                    observer.OnNext(value);
                }

                return this;
            }

            public void Dispose() { }
        }
    }
}
