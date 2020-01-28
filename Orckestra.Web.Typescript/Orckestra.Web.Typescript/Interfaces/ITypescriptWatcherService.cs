using System;
using System.Collections.Generic;

namespace Orckestra.Web.Typescript.Interfaces
{
    public interface ITypescriptWatcherService
    {
        ITypescriptWatcherService ConfigureService(Action watchAction, string fileMask, IEnumerable<string> paths);
        ITypescriptWatcherService InvokeService(); 
    }
}