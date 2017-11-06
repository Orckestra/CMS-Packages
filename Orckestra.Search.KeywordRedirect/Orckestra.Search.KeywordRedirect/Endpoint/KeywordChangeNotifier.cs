using System;
using System.Collections.Generic;
using System.Globalization;
using Composite.Core.Application;
using Composite.Data;
using Composite.Data.ProcessControlled;
using Microsoft.Extensions.DependencyInjection;

namespace Orckestra.Search.KeywordRedirect.Endpoint
{

    [ApplicationStartup()]
    internal class KeywordChangeNotifierRegistrator
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(KeywordChangeNotifier));
        }
    }

    /// <summary>
    /// Component change structure
    /// </summary>
    public class KeywordChange
    {
        /// <summary>
        /// CultureInfo
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

    }
    public class KeywordChangeNotifier : IObservable<KeywordChange>
    {

        public KeywordChangeNotifier()
        {
            _observers = new List<IObserver<KeywordChange>>();
        }

        private readonly List<IObserver<KeywordChange>> _observers;

        public IDisposable Subscribe(IObserver<KeywordChange> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<KeywordChange>> _observers;
            private readonly IObserver<KeywordChange> _observer;

            public Unsubscriber(List<IObserver<KeywordChange>> observers, IObserver<KeywordChange> observer)
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
        /// Notify a change in keywords
        /// </summary>
        public void KeywordChange(object sender, DataEventArgs dataEventArgs)
        {
            var localizedControlled = dataEventArgs.Data as ILocalizedControlled;
            if (localizedControlled != null)
            {
                var change = new KeywordChange { CultureInfo = new CultureInfo(localizedControlled.SourceCultureName) };
                foreach (var observer in _observers)
                {
                    observer.OnNext(change);
                }
            }
        }
    }
}
