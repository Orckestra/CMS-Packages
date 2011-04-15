using System;
using System.Collections.Generic;

namespace Composite.Tools.PackageCreator
{
	public class GuidReplacer : IDisposable
	{
		private int count = 0;
		public GuidReplacer(Guid replace, Guid need)
		{
			Stack.Push(new KeyValuePair<Guid, Guid>(replace, need));
			count++;
		}

		public GuidReplacer(IEnumerable<KeyValuePair<Guid, Guid>> list)
		{
			foreach (var item in list)
			{
				Stack.Push(item);
				count++;
			}
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

		[ThreadStatic]
		private static Stack<KeyValuePair<Guid, Guid>> stack;

		public static Stack<KeyValuePair<Guid, Guid>> Stack
		{
			get
			{
				if (stack == null)
					stack = new Stack<KeyValuePair<Guid, Guid>>();
				return stack;
			}
		}

		[ThreadStatic]
		public static Guid CompositionContainerGuid = Guid.NewGuid();

		public void Dispose()
		{
			for (int i = 0; i < count; i++)
			{
				Stack.Pop();
			}

		}
	}

}
