using System;
using System.Linq.Expressions;
using Avalonia.Data.Core.Parsers;

#nullable enable

namespace Avalonia.Data.Core
{
    public static class BindingExpression
    {
        public static BindingExpression<TIn, TOut> OneWay<TIn, TOut>(
            TIn root,
            Expression<Func<TIn, TOut>> read)
                where TIn : class
        {
            return OneWay(new Single<TIn>(root), read);
        }

        public static BindingExpression<TIn, TOut> OneWay<TIn, TOut>(
            IObservable<TIn> root,
            Expression<Func<TIn, TOut>> read)
                where TIn : class
        {
            return new BindingExpression<TIn, TOut>(
                root,
                read.Compile(),
                null,
                ExpressionChainVisitor.Build(read));
        }

        public static BindingExpression<TIn, TConverted> OneWay<TIn, TOut, TConverted>(
            TIn root,
            Expression<Func<TIn, TOut>> read,
            Func<TOut, TConverted> convert)
                where TIn : class
        {
            return OneWay(new Single<TIn>(root), read, convert);
        }

        public static BindingExpression<TIn, TConverted> OneWay<TIn, TOut, TConverted>(
            IObservable<TIn> root,
            Expression<Func<TIn, TOut>> read,
            Func<TOut, TConverted> convert)
                where TIn : class
        {
            var compiledRead = read.Compile();

            return new BindingExpression<TIn, TConverted>(
                root,
                x => convert(compiledRead(x)),
                null,
                ExpressionChainVisitor.Build(read));
        }

        public static BindingExpression<TIn, TOut> TwoWay<TIn, TOut>(
            TIn root,
            Expression<Func<TIn, TOut>> read,
            Action<TIn, TOut> write)
                where TIn : class
        {
            return TwoWay(new Single<TIn>(root), read, write);
        }

        public static BindingExpression<TIn, TOut> TwoWay<TIn, TOut>(
            IObservable<TIn> root,
            Expression<Func<TIn, TOut>> read,
            Action<TIn, TOut> write)
                where TIn : class
        {
            return new BindingExpression<TIn, TOut>(
                root,
                read.Compile(),
                write,
                ExpressionChainVisitor.Build(read));
        }

        public static BindingExpression<TIn, TConverted> TwoWay<TIn, TOut, TConverted>(
            TIn root,
            Expression<Func<TIn, TOut>> read,
            Action<TIn, TOut> write,
            Func<TOut, TConverted> convert,
            Func<TConverted, TOut> convertBack)
                where TIn : class
        {
            return TwoWay(new Single<TIn>(root), read, write, convert, convertBack);
        }

        public static BindingExpression<TIn, TConverted> TwoWay<TIn, TOut, TConverted>(
            IObservable<TIn> root,
            Expression<Func<TIn, TOut>> read,
            Action<TIn, TOut> write,
            Func<TOut, TConverted> convert,
            Func<TConverted, TOut> convertBack)
                where TIn : class
        {
            var compiledRead = read.Compile();

            return new BindingExpression<TIn, TConverted>(
                root,
                x => convert(compiledRead(x)),
                (o, v) => write(o, convertBack(v)),
                ExpressionChainVisitor.Build(read));
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
