using System;
using System.Collections.Generic;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.Core.PackageSystem;

namespace PackageFragmentInstallers
{
	public class FixUserSettingsPackageFragmentUninstaller : BasePackageFragmentUninstaller
	{
		public override void Uninstall()
		{
			throw new InvalidOperationException("You can't uninstall this package");
		}

		public override IEnumerable<PackageFragmentValidationResult> Validate()
		{
			yield return new PackageFragmentValidationResult(PackageFragmentValidationResultType.Fatal, "Uninstall not supported");
		}
	}
}
