using System;
using System.IO;
using System.Xml.Linq;
using Composite.Core.IO;

namespace Composite.Forms.FormSubmitHandler
{
	public static class StoreHelper
	{
		static StoreHelper()
		{
			StoreHelper.FormSubmit10 = "http://www.composite.net/ns/form/submit/handler/1.0";
		}

		public static XNamespace FormSubmit10 { get; private set; }

		private static readonly string _storingPath = "App_Data/FormSubmitHandler";
		public static string StoringPath { get { return _storingPath; } }

		private static object _lock = new object();

		public static void Save(string FormId, XElement Result)
		{
			var date = DateTime.Now;
			var directory = Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), _storingPath);
			var formDirectory = Path.Combine(directory, Helper.ToValidIndefiniter(FormId));
			
			Result.Add(new XAttribute(FormSubmit10 + "FormId", FormId));
			Result.Add(new XAttribute(FormSubmit10 + "Date", date));
	
			lock (_lock)
			{
				if(!Directory.Exists(formDirectory))
					Directory.CreateDirectory(formDirectory);
				string formFileName = string.Format("{0:yyyyMM}.xml", date);

				string formFilePath = Path.Combine(formDirectory, formFileName);

				XElement storedData = GetDataXml(formFilePath);
				storedData.Add(Result);
				storedData.Save(formFilePath);

			}
		}

		private static XElement GetDataXml(string formFilePath)
		{
			XElement results = new XElement("Results");
			try
			{
				results = XElement.Load(formFilePath);
			}
			catch
			{
			}
			return results;
		}
	}
}