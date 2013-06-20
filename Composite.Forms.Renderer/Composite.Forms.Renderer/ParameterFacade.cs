using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Composite.Core.Types;
using Composite.Data;

namespace Composite.Forms.Renderer
{
    public class ParameterFacade
    {
        private static readonly XNamespace data = "http://www.composite.net/ns/dynamicdata/1.0";


        public static string GetProperty(string propertyName)
        {
            var result = new XElement(data + "fieldreference"
                                      , new XAttribute(XNamespace.Xmlns + "data", data)
                                      , new XAttribute("fieldname", propertyName)
                );
            return result.ToString();
        }

        internal static void ResolveProperties(FormEmail formEmail)
        {
            formEmail.From = ResolveProperties(formEmail.From);
            formEmail.To = ResolveProperties(formEmail.To);
            formEmail.Cc = ResolveProperties(formEmail.Cc);
        }

        internal static string ResolveProperties(string content)
        {
            //TODO: resolve more complex content
            XElement fieldreference = XElementTryParse(content);

            if (fieldreference != null)
            {
                IData currentData = FormsRendererDataScope.CurrentData;

                string propertyName = fieldreference.Attributes("fieldname").Select(d => d.Value).FirstOrDefault();

                if (currentData != null && fieldreference.Name == data + "fieldreference" && propertyName != null)
                {
                    PropertyInfo propertyInfo = currentData.DataSourceId.InterfaceType.GetProperty(propertyName);

                    if (propertyInfo == null)
                    {
                        throw new InvalidOperationException(
                            string.Format("Datatype '{0}.{1}' does not have property '{2}'",
                                          currentData.DataSourceId.InterfaceType.Namespace,
                                          currentData.DataSourceId.InterfaceType.Name, propertyName));
                    }

                    object value = propertyInfo.GetValue(currentData, null);

                    List<ForeignKeyAttribute> foreignKeyAttributes =
                        propertyInfo.GetCustomAttributesRecursively<ForeignKeyAttribute>().ToList();
                    if (foreignKeyAttributes.Count > 0)
                    {
                        IData foreignData = currentData.GetReferenced(propertyInfo.Name);
                        value = foreignData.GetLabel();
                    }
                    return (value ?? string.Empty).ToString();
                }
            }
            return content;
        }

        private static XElement XElementTryParse(string text)
        {
            try
            {
                return XElement.Parse(text);
            }
            catch (XmlException)
            {
                return null;
            }
        }
    }
}