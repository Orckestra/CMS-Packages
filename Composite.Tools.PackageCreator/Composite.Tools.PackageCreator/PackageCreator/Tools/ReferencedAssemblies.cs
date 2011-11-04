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

		public static int AssemblyPosition(XElement file)
		{
			var path = file.AttributeValue("targetFilename");
			var filename = Path.GetFileName(path);
			return AssemblyPosition(filename);
		}

		public static int AssemblyPosition(string name)
		{
			if (Referenced.ContainsKey(name))
			{
				return Referenced[name].Select(d => AssemblyPosition(d)).Max() + 1;
			}
			return 0;
		}
	}
}
