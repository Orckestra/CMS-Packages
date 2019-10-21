using System;
using System.Collections.Generic;
using System.Globalization;
using Composite.Core.Application;
using Composite.Data;
using Composite.Data.ProcessControlled;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Search.KeywordRedirect.Data.Types;

namespace Orckestra.Search.KeywordRedirect.Endpoint
{
    [ApplicationStartup()]
    internal class BeforeKeywordChangeNotifierRegistrator
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(BeforeKeywordChangeNotifier));
        }
    }

    public class BeforeKeywordChangeNotifier : IObservable<RedirectKeyword>
    {

        public BeforeKeywordChangeNotifier()
        {
            _observers = new List<IObserver<RedirectKeyword>>();
        }

        private readonly List<IObserver<RedirectKeyword>> _observers;

        public IDisposable Subscribe(IObserver<RedirectKeyword> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<RedirectKeyword>> _observers;
            private readonly IObserver<RedirectKeyword> _observer;

            public Unsubscriber(List<IObserver<RedirectKeyword>> observers, IObserver<RedirectKeyword> observer)
            {
                this._observers = observers;
                this._observer = observer;
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

        /// <summary>
        /// Notify before change in keywords
        /// </summary>
        public void BeforeKeywordChange(object sender, DataEventArgs dataEventArgs)
        {
            if (dataEventArgs.Data is RedirectKeyword redirectKeyword)
            {
                foreach (var observer in _observers)
                {
                    observer.OnNext(redirectKeyword);
                }
            }
        }
    }
}
