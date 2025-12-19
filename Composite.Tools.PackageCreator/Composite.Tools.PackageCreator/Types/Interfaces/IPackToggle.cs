using Composite.C1Console.Elements;
using Composite.C1Console.Security;

namespace Composite.Tools.PackageCreator.Types
{
    internal interface IPackToggle
    {
        ActionCheckedStatus CheckedStatus { get; }

        bool Disabled { get; }

        EntityToken GetEntityToken();
    }
}