using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Data;

namespace Composite.Forms.Renderer
{
    public class FormsRendererDataScope : IDisposable
    {
        [ThreadStatic]
        private static Stack<IData> list;
        private static Stack<IData> List
        {
            get
            {
                if (list == null)
                {
                    list = new Stack<IData>();
                }
                return list;
            }
        }

        public static IData CurrentData
        {
            get
            {
                if (List.Count == 0)
                    return null;
                return List.Peek();
            }
        }

        public FormsRendererDataScope(IData data)
        {
            List.Push(data);
        }

        #region IDisposable Members

        public void Dispose()
        {
            List.Pop();
        }

        #endregion
    }
}
