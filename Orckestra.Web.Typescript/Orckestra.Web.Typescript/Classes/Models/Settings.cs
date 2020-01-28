using System.Collections.Generic;
using System.Xml.Serialization;

namespace Orckestra.Web.Typescript.Classes.Models
{
    [XmlRoot("settings")]
    public class Settings
    {
        [XmlElement("compilerTimeOutSeconds")]
        public int CompilerTimeOutSeconds { get; set; }

        [XmlArray("typescriptTasks")]
        [XmlArrayItem("typescriptTask")]
        public List<TypescriptTask> TypescriptTasks { get; set; }
    }
}