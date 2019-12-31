using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace Composite.Web.Js.TypeScript
{
    public class TypeScriptConfig
    {
        public static TypeScriptConfig Get()
        {
            var section = ConfigurationManager.GetSection("typescript/settings") as NameValueCollection;
            if (section == null)
                throw new Exception("Configuration section 'typescript/settings' is missing");

            var bundleLocation = section[nameof(BundleLocation)];
            if (string.IsNullOrEmpty(bundleLocation))
                throw new Exception("'BundleLocation' configuration is missing");

            var minify = section[nameof(Minify)];

            var config = new TypeScriptConfig
            {
                BundleLocation = bundleLocation,
                Minify = !string.IsNullOrEmpty(minify) && bool.Parse(minify),
            };

            var keys = section.AllKeys.Where(x => x != nameof(BundleLocation) && x != nameof(Minify));
            var options = new List<string>();
            foreach (var key in keys)
            {
                options.Add($" {key} {section[key]} ");
            }

            config.Options = options.AsReadOnly();

            return config;
        }

        public string BundleLocation { get; private set; }
        public bool Minify { get; private set; }
        public ICollection<string> Options { get; private set; }

        private TypeScriptConfig()
        {
        }
    };
}