using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Composite.Tools.PackageCreator
{
    public class ReferencedAssemblies
    {
        public static object Lock = new object();

        private static Dictionary<string, HashSet<string>> _referenced;

        private static Dictionary<string, HashSet<string>> Referenced
        {
            get
            {
                lock (Lock)
                {
                    if (_referenced != null)
                        return _referenced;

                    _referenced = new Dictionary<string, HashSet<string>>();

                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (!assembly.IsBinFolder())
                            continue;

                        var name = assembly.ManifestModule.Name;

                        foreach (var raName in assembly.GetReferencedAssemblies())
                            try
                            {
                                var ra = Assembly.Load(raName);
                                if (ra != null && ra.IsBinFolder()) _referenced.Add(name, ra.ManifestModule.Name);
                            }
                            catch
                            {
                                // ignored
                            }
                    }

                    return _referenced;
                }
            }
        }

        public static int AssemblyPosition(XElement element)
        {
            var targetFilename = element.AttributeValue("targetFilename");
            if (targetFilename != null)
                return AssemblyPosition(Path.GetFileName(targetFilename));

            var path = element.AttributeValue("path");
            return path != null ? AssemblyPosition(Path.GetFileName(path)) : 0;
        }

        public static int AssemblyPosition(string name, string[] chain = null)
        {
            if (name.Equals("Composite.Generated.dll", StringComparison.CurrentCultureIgnoreCase))
                return 0;

            if (chain != null && chain.Contains(name))
                return int.MaxValue / 2;

            if (!Referenced.ContainsKey(name))
                return 0;

            var newChain = (chain ?? new string[] { }).Concat(new[] { name }).ToArray();
            return Referenced[name].Select(d => AssemblyPosition(d, newChain)).Max() + 1;
        }
    }
}