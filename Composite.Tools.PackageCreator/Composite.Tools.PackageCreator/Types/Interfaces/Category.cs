using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Composite.Tools.PackageCreator.Types
{
	[AttributeUsage(AttributeTargets.Class| AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public sealed class PCCategoryAttribute : Attribute
	{
		internal PCCategoryAttribute(string name){
			this.Name = name;
			this.Label = string.Empty;
		}

		public PCCategoryAttribute(string name, string label)
		{
			this.Name = name;
			this.Label = label;
		}

		public string Name {
			get;
			private set;
		}

		private string Label
		{
			get;
			set;
		}

		public string CommonName
		{
			get;
			set;
		}

		public string[] AliasNames
		{
			get;
			set;
		}

		public string GetLabel()
		{
			if(string.IsNullOrEmpty(Label))
				return PackageCreatorFacade.GetLocalization( string.Format("{0}.Label", Name));
			return Label;
		}
	}
	
}
