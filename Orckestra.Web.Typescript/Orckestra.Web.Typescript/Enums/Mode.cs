using System.Xml.Serialization;

namespace Orckestra.Web.Typescript.Enums
{
    public enum Mode
    {
        [XmlEnum("once")]
        Once,
        [XmlEnum("dynamic")]
        Dynamic,
        [XmlEnum("off")]
        Off
    }
}