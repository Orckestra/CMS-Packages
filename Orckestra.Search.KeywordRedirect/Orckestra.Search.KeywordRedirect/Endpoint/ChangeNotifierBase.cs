using System;
using System.Collections.Generic;
using Composite.Data;

namespace Orckestra.Search.KeywordRedirect.Endpoint
{
    public abstract class ChangeNotifierBase<T> : IObservable<T>
    {
        protected ChangeNotifierBase()
        {
            Observers = new List<IObserver<T>>();
        }

        protected List<IObserver<T>> Observers { get; }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!Observers.Contains(observer))
                Observers.Add(observer);
            return new Unsubscriber(Observers, observer);
        }

        internal class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
#if LeakCheck
                GC.SuppressFinalize(this);
#endif
            }

#if LeakCheck
            private string stack = Environment.StackTrace;
            /// <exclude />
            ~Unsubscriber()
            {
                Composite.Core.Instrumentation.DisposableResourceTracer.RegisterFinalizerExecution(stack);
            }
#endif
        }

        public virtual void OnChange(object sender, DataEventArgs dataEventArgs)
        {
            if (dataEventArgs.Data is T data)
            {
                foreach (var observer in Observers)
                {
                    observer.OnNext(data);
                }
            }
        }
    };
}