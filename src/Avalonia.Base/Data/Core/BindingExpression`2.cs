using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Subjects;
using Avalonia.Reactive;

#nullable enable

namespace Avalonia.Data.Core
{
    public class BindingExpression<TIn, TOut> : LightweightObservableBase<BindingValue<TOut>>,
        ISubject<TIn, BindingValue<TOut>>,
        IDescription
            where TIn : class
    {
        private readonly IObservable<TIn> _rootSource;
        private readonly Func<TIn, TOut> _read;
        private readonly Link[] _chain;
        private IDisposable? _rootSourceSubsciption;
        private WeakReference<TIn>? _root;

        public BindingExpression(
            IObservable<TIn> root,
            Func<TIn, TOut> read,
            List<Delegate> links)
        {
            _rootSource = root;
            _read = read;
            _chain = new Link[links.Count];

            for (var i = 0; i < links.Count; ++i)
            {
                _chain[i] = new Link(links[i]);
            }
        }

        public string Description => "TODO";

        void IObserver<TIn>.OnCompleted()
        {
        }

        void IObserver<TIn>.OnError(Exception error)
        {
        }

        void IObserver<TIn>.OnNext(TIn value) => throw new NotImplementedException();

        protected override void Initialize()
        {
            _rootSourceSubsciption = _rootSource.Subscribe(RootChanged);
        }

        protected override void Deinitialize()
        {
            StopListeningToChain(0);
            _rootSourceSubsciption?.Dispose();
        }

        protected override void Subscribed(IObserver<BindingValue<TOut>> observer, bool first)
        {
            if (!first && _root != null && _root.TryGetTarget(out var root))
            {
                observer.OnNext(GetResult(root));
            }
        }

        private void RootChanged(TIn value)
        {
            _root = new WeakReference<TIn>(value);

            if (value != null)
            {
                ListenToChain(0);
                PublishValue();
            }
        }

        private void ListenToChain(int from)
        {
            if (_root != null && _root.TryGetTarget(out var root))
            {
                var arg = new[] { root };
                object? last = null;

                try
                {
                    for (var i = from; i < _chain.Length; ++i)
                    {
                        var o = _chain[i].Eval.DynamicInvoke(arg);

                        if (o != last)
                        {
                            _chain[i].Value = o;

                            if (SubscribeToChanges(o))
                            {
                                last = o;
                            }
                        }
                    }
                }
                catch
                {
                    // Broken expression chain.
                }
            }
        }

        private void StopListeningToChain(int from)
        {
            if (_root != null && _root.TryGetTarget(out var root))
            {
                var arg = new[] { root };

                for (var i = from; i < _chain.Length; ++i)
                {
                    UnsubscribeToChanges(_chain[i].Value);
                }
            }
        }

        private bool SubscribeToChanges(object? o)
        {
            if (o is null)
            {
                return false;
            }

            var result = false;

            if (o is IAvaloniaObject ao)
            {
                ao.PropertyChanged += ChainPropertyChanged;
                result |= true;
            }
            else if (o is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += ChainPropertyChanged;
                result |= true;
            }

            if (o is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += ChainCollectionChanged;
                result |= true;
            }

            return result;
        }

        private void UnsubscribeToChanges(object? o)
        {
            if (o is null)
            {
                return;
            }

            if (o is IAvaloniaObject ao)
            {
                ao.PropertyChanged -= ChainPropertyChanged;
                return;
            }
            else if (o is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged -= ChainPropertyChanged;
            }

            if (o is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged -= ChainCollectionChanged;
            }
        }

        private BindingValue<TOut> GetResult(TIn root)
        {
            try
            {
                var value = _read(root);
                return new BindingValue<TOut>(value);
            }
            catch (Exception e)
            {
                return new BindingValue<TOut>(e);
            }
        }

        private void PublishValue()
        {
            if (_root != null && _root.TryGetTarget(out var root))
            {
                var result = GetResult(root);
                PublishNext(result);
            }
        }

        private void ChainPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PublishValue();
        }

        private void ChainPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            PublishValue();
        }

        private void ChainCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PublishValue();
        }

        private struct Link
        {
            public Link(Delegate eval)
            {
                Eval = eval;
                Value = null;
            }

            public Delegate Eval { get; }
            public object? Value; // TODO: WeakReference
        }
    }
}
