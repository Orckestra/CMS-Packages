using Orckestra.Web.Typescript.Enums;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Orckestra.Web.Typescript.Classes.Models
{
    [XmlRoot("typescriptTask")]
    public class TypescriptTask
    {
        [XmlElement("mode")]
        public Mode Mode { get; set; }

        [XmlElement("cancelIfOutFileExist")]
        public bool CancelIfOutFileExist { get; set; }

        [XmlArray("pathsToWatch")]
        [XmlArrayItem("pathToWatch")]
        public List<string> PathsToWatch { get; set; }

        [XmlElement("pathTypescriptConfigFile")]
        public string PathTypescriptConfigFile { get; set; }

        [XmlElement("fileMask")]
        public string FileMask { get; set; }

        [XmlElement("minification")]
        public Minification Minification { get; set; }
    }
}
