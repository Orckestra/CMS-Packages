using System.Collections.Generic;
using System.Xml.Serialization;

namespace Orckestra.Web.Typescript.Classes.Models
{
    [XmlRoot("settings")]
    public class Settings
    {
        [XmlArray("typescriptTasks")]
        [XmlArrayItem("typescriptTask")]
        public List<TypescriptTask> TypescriptTasks { get; set; }
    }
}