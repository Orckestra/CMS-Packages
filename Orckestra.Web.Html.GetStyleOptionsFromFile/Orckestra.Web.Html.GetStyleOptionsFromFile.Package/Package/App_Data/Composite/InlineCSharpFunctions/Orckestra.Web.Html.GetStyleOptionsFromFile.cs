using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.Core;

namespace Orckestra.Web.Html
{
    public static class InlineMethodFunction
    {
        public static Dictionary<string, string> GetStyleOptionsFromFile(string OptionsXMLFilePath)
        {
            var options = new Dictionary<string, string>();
            try
            {
                var optionsFile = XDocument.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, OptionsXMLFilePath));
                options = optionsFile.Root.Elements("Option").ToDictionary(el => el.Attribute("CssClassKey").Value, el => el.Attribute("Label").Value);
            }
            catch (Exception ex)
            {
                Log.LogError("Error while loading style options", ex);
            }
            return options;
        }

    }
}
