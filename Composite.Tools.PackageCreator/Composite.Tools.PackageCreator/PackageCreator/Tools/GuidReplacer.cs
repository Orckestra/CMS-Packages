using System;
using System.Collections.Generic;

namespace Composite.Tools.PackageCreator
{
    public class GuidReplacer : IDisposable
    {
        [ThreadStatic] private static Stack<KeyValuePair<Guid, Guid>> _stack;

        [ThreadStatic] public static Guid CompositionContainerGuid = Guid.NewGuid();

        private readonly int _count;

        public GuidReplacer(Guid replace, Guid need)
        {
            Stack.Push(new KeyValuePair<Guid, Guid>(replace, need));
            _count++;
        }

        public GuidReplacer(IEnumerable<KeyValuePair<Guid, Guid>> list)
        {
            foreach (var item in list)
            {
                Stack.Push(item);
                _count++;
            }
        }

        public static Stack<KeyValuePair<Guid, Guid>> Stack =>
            _stack ?? (_stack = new Stack<KeyValuePair<Guid, Guid>>());

        public void Dispose()
        {
            for (var i = 0; i < _count; i++) Stack.Pop();
        }

        public static Guid Replace(Guid guid)
        {
#warning #3102 Disable GuidReplacer
            return guid;

            //if (guid == Guid.Empty)
            //    return guid;
            //foreach (var rule in Stack)
            //{
            //    if (guid == rule.Key)
            //        return rule.Value;
            //}
            //return guid;
        }
    }
}