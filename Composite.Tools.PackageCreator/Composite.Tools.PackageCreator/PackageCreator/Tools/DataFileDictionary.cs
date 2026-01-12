using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Composite.Tools.PackageCreator
{
    internal class DataFileDictionary : Dictionary<string, XElement>
    {
        public new void Add(string fileName, XElement addElement)
        {
            if (!ContainsKey(fileName)) 
                this[fileName] = new XElement("Data");

            this[fileName].Add(addElement);
        }

        public void Save()
        {
            foreach (var fileName in Keys)
            {
                var targetDirectory = Path.GetDirectoryName(fileName);
                if (targetDirectory == null) 
                    continue;

                if (!Directory.Exists(targetDirectory)) 
                    Directory.CreateDirectory(targetDirectory);

                this[fileName].Save(fileName);
            }
        }
    }
}