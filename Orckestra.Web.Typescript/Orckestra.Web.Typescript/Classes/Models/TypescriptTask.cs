using Orckestra.Web.Typescript.Interfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Orckestra.Web.Typescript.Classes.Models
{
    [XmlRoot("typescriptTask")]
    public class TypescriptTask
    {
        [XmlElement("taskName")]
        public string TaskName { get; set; }

        [XmlElement("allowOverwrite")]
        public bool AllowOverwrite { get; set; }

        [XmlArray("pathsToWatchForChanges")]
        [XmlArrayItem("pathToWatchForChanges")]
        public List<string> PathsToWatchForChanges { get; set; }

        [XmlElement("pathTypescriptConfigFile")]
        public string PathTypescriptConfigFile { get; set; }

        [XmlElement("compilerTimeOutSeconds")]
        public int CompilerTimeOutSeconds { get; set; }

        [XmlElement("fileMask")]
        public string FileMask { get; set; }

        [XmlElement("useMinification")]
        public bool UseMinification { get; set; }

        [XmlElement("minifiedFileName")]
        public string MinifiedFileName { get; set; }
    }
}