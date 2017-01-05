using System.Collections.Generic;
using System.Linq;
using Composite.Core.PackageSystem;
using Composite.Core.PackageSystem.PackageFragmentInstallers;

namespace Orckestra.Search.Package
{
    public class AddDefaultUserGroupPermissionsUninstaller: BasePackageFragmentUninstaller
    {
        public override IEnumerable<PackageFragmentValidationResult> Validate()
        {
            return Enumerable.Empty<PackageFragmentValidationResult>();
        }

        public override void Uninstall()
        {
        }
    }
}
