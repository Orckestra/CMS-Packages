using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.IO;

namespace Composite.Tools.PackageCreator
{
	public class ReferencedAssemblies
	{
		public static object _lock = new object();

		private static Dictionary<string, HashSet<string>> _referenced;
		private static Dictionary<string, HashSet<string>> Referenced{
			get{
				lock (_lock)
				{
					if (_referenced == null)
					{
						_referenced = new Dictionary<string, HashSet<string>>();
						foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
						{
							if (assembly.IsBinFolder())
							{
								var name = assembly.ManifestModule.Name;

								foreach (var raName in assembly.GetReferencedAssemblies())
								{
									var ra = Assembly.Load(raName);
									if (ra != null && ra.IsBinFolder())
									{
										_referenced.Add(name, ra.ManifestModule.Name);
									}
								}
							}

						}

					}
					return _referenced;
				}
			}
		}

		public static int AssemblyPosition(XElement element)
		{

			var targetFilename = element.AttributeValue("targetFilename");
			if(targetFilename != null)
				return AssemblyPosition(Path.GetFileName(targetFilename));

			var path = element.AttributeValue("path");
			if(path != null)
				return AssemblyPosition(Path.GetFileName(path));
			
			return 0;
		}

		public static int AssemblyPosition(string name, string[] chain = null)
		{
			if (name.Equals("Composite.Generated.dll", StringComparison.CurrentCultureIgnoreCase))
				return 0;

			if (chain != null && chain.Contains(name))
				return int.MaxValue/2;

			if (Referenced.ContainsKey(name))
			{
				var newchain = (chain ?? new string[]{}).Concat(new[]{name}).ToArray();
				return Referenced[name].Select(d => AssemblyPosition(d, newchain)).Max() + 1;
			}
			return 0;
		}
	}
}
