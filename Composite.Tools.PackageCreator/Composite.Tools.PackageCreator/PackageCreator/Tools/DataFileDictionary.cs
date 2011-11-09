using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Composite.Tools.PackageCreator
{
	internal class DataFileDictionary: Dictionary<string, XElement>
	{
		public new void Add(string fileName, XElement addElement)
		{
			if (!ContainsKey(fileName))
			{
				this[fileName] = new XElement("Data");
			}
			this[fileName].Add(addElement);
		}

		public void Save()
		{
			foreach (var fileName in this.Keys)
			{
				string targetDirectory = Path.GetDirectoryName(fileName);
				if (targetDirectory == null) continue;
				if (Directory.Exists(targetDirectory) == false)
				{
					Directory.CreateDirectory(targetDirectory);
				}
				this[fileName].Save(fileName);
			}
		}
	}
}
