using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;
using Composite.Data;
using Newtonsoft.Json;

namespace Composite.AspNet.WebAPI
{
    internal class CustomIDataXmlFormatter : MediaTypeFormatter
    {
        public CustomIDataXmlFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(IData).IsAssignableFrom(type) || typeof(IEnumerable<IData>).IsAssignableFrom(type);
        }

        public override Task WriteToStreamAsync(Type type, object value,
            Stream writeStream, System.Net.Http.HttpContent content,
            System.Net.TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                if (typeof (IEnumerable<IData>).IsAssignableFrom(type))
                {
                    Type elementType = type.GetGenericArguments().First();

                    var resultXElement = new XElement("Data");

                    foreach (var element in (value as IEnumerable))
                    {
                        resultXElement.Add(GetXNode(elementType.Name, element).Root);
                    }

                    resultXElement.Save(writeStream);
                    return;
                }

                XDocument xNode = GetXNode(type.Name, value);

                xNode.Save(writeStream);
            });
        }

        private XDocument GetXNode(string name, object value)
        {
            var json = JsonConvert.SerializeObject(value, GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);

            var xNode = JsonConvert.DeserializeXNode(string.Format("{{\"{0}\": {1} }}", name, json));

            return xNode;
        }
    }
}
