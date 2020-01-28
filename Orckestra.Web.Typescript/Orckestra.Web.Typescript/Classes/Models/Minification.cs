using System.Xml.Serialization;

namespace Orckestra.Web.Typescript.Classes.Models
{
    [XmlRoot("minification")]
    public class Minification
    {
        [XmlElement("useMinification")]
        public bool UseMinification { get; set; }

        [XmlElement("minifiedFileName")]
        public string MinifiedFileName { get; set; }
    }
}
