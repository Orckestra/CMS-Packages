using System.Collections.Generic;
using Composite.C1Console.Security;

namespace Composite.Tools.PackageCreator.Types
{

    [PackCategory("FilesInDirectories")]
    internal class PCFilesInDirectory : BasePackItem
    {
        public PCFilesInDirectory(string name)
            : base(name)
        {
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            yield break;
        }
    }
}
