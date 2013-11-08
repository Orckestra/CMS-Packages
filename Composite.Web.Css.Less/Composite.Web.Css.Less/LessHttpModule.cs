using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Composite.C1Console.Security;
using Composite.Core.Collections.Generic;

namespace Composite.Web.Css.Less
{
    public class LessHttpModule : CssCompilationHttpModule
    {
        public LessHttpModule() : base(".less", "*.less", CompressFiles.CompressLess)
        {
        }
    }
}