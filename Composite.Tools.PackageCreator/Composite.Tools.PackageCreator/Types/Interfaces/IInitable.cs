using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Composite.Tools.PackageCreator.Types
{
	public interface IInitable
	{
		void Init(PackageCreator creator);
	}
}
