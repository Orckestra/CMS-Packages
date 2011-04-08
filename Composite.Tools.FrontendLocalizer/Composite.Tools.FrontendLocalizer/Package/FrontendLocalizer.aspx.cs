using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using Composite.Data;
using Composite.Data.Plugins.DataProvider.Streams;
using Composite.Data.Types;
using Composite.Core.IO;
using Composite.C1Console.Security;
using ICSharpCode.SharpZipLib.Zip;

namespace Frontend
{
	public partial class FrontendLocalizer : System.Web.UI.Page
	{

		private const string _relativeBackupDirectory = "App_Data/Backups/FrontendLocalizer";
		protected string NewBackupFilePath
		{
			get
			{
				var backupDirectory = Path.Combine(PathUtil.BaseDirectory, _relativeBackupDirectory);
				return Path.Combine(backupDirectory, string.Format("FrontendLocalizer_{0}.zip", DateTime.Now.ToString("yyyy_MM.dd_HHmm")));
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!UserValidationFacade.IsLoggedIn())
			{
				Response.Redirect(string.Format("Composite/Login.aspx?ReturnUrl={0}", HttpUtility.HtmlEncode(Request.Url.PathAndQuery)));
			}
		}

		protected void Localize_Click(object sender, EventArgs e)
		{
			try
			{

				using (var resourceFile = new ResourceFile(txtFileName.Text))
				using (new Backuper(NewBackupFilePath))
				{

					foreach (var layout in DataFacade.GetData<IPageTemplate>())
					{
						string templatePath = layout.PageTemplateFilePath;
						resourceFile.LocalizeFile<IPageTemplateFile>(templatePath);
					}
					foreach (var function in DataFacade.GetData<IXsltFunction>())
					{
						string path = function.XslFilePath;
						resourceFile.LocalizeFile<IXsltFile>(path);
					}
					txtFileName.Visible = false;
					valFileName.Visible = false;
					lblFileName.Visible = true;
					lblFileName.Text = txtFileName.Text;
					Localize.Visible = false;
					lblFileNameLabel.Text = "Please find the resource file here:";
					lblResult.Text = "Localization completed!";
				}
			}
			catch (Exception ex)
			{
				this.Validators.Add(new ErrorSummary(ex.Message));
			}
		}
	}

	public static class FrontendLocalizerExtension
	{
		private static XNamespace lang = "http://www.composite.net/ns/localization/1.0";
		private static XNamespace xmlns = "http://www.w3.org/1999/xhtml";
		public static void LocalizeFile<T>(this ResourceFile resourceFile, string filePath) where T : class, IFile
		{
			T file = IFileServices.TryGetFile<T>(filePath);

			var fileSystemFile = file as FileSystemFileBase;
			if (fileSystemFile != null)
			{
				Backuper.Current.Add(fileSystemFile.SystemPath);
			}

			XDocument xdoc = XDocument.Parse(file.ReadAllText());

			List<XText> textNodes = xdoc.DescendantNodes().Where(f => f is XText).Cast<XText>().ToList();
			foreach (XText textNode in textNodes)
			{
				string theText = textNode.Value;
				if(Regex.Match(textNode.Value.Trim(),@"^\d+$").Success)
					continue;
				if (SomeParentIs(textNode, "style", 3))
					continue;
				if (SomeParentIs(textNode, "script", 3))
					continue;
				if (SomeParentIs(textNode, "attribute", 3))
					continue;
				if(textNode.Value.Trim().Equals("True", StringComparison.CurrentCultureIgnoreCase))
					continue;
				if(textNode.Value.Trim().Equals("False", StringComparison.CurrentCultureIgnoreCase))
					continue;
				if (SomeContainValue(textNode, "Renderers/Page.aspx", 3))
					continue;
				if (SomeContainValue(textNode, "Renderers/ShowMedia.ashx", 3))
					continue;
				//if (Regex.Match(textNode.Value, @"background[\-\w]*url\s*\()").Success)
				//	continue;

				var trimText = theText.Trim();
				if(string.IsNullOrEmpty(trimText))
					continue;

				var beforeTrim = theText.Substring(0, theText.IndexOf(trimText));
				var afterTrim = theText.Substring(theText.IndexOf(trimText) + trimText.Length);  

				var key = resourceFile.AddString(trimText);
				if (key != null)
					textNode.ReplaceWith(
						string.IsNullOrEmpty(beforeTrim)? null : new XText(beforeTrim),
						new XElement(lang + "string", new XAttribute("key", key)),
						string.IsNullOrEmpty(afterTrim)? null : new XText(afterTrim)
					);
			}
			file.SetNewContent(xdoc.ToString());
			DataFacade.Update(file);
		}
		public static bool SomeParentIs(XText textNode, string name, int depth)
		{
			XElement parent = textNode.Parent;
			while (parent != null && depth-- > 0)
			{
				if (parent.Name.LocalName.ToLower() == name)
				{
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}

		public static bool SomeContainValue(XText textNode, string name, int depth)
		{
			XElement parent = textNode.Parent;
			while (parent != null && depth-- > 0)
			{
				if (parent.Value.Contains(name))
				{
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}

	}

	public class ResourceFile : IDisposable
	{
		private string _filename;
		private string resoursePath;
		private TextInfo _textInfo = new CultureInfo("en-US", false).TextInfo;
		private Dictionary<string, string> localization = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

		public ResourceFile(string filename)
		{
			resoursePath = HttpContext.Current.Server.MapPath(string.Format("~/App_GlobalResources/{0}.resx", filename));
			if (File.Exists(resoursePath))
			{
				throw new InvalidOperationException("The file you have specified already exists. Please enter another file name.");
			}
			_filename = filename;

		}

		/// <summary>
		/// Add localization string
		/// </summary>
		/// <param name="text"></param>
		/// <returns>Key</returns>
		public string AddString(string text)
		{
			var key = _textInfo.ToTitleCase(text);
			key = Regex.Replace(key, "[^a-zA-Z0-9]", "");
			key = key.Substring(0, key.Length > 20 ? 20 : key.Length);
			if (string.IsNullOrEmpty(key))
				return null;
			if (Char.IsDigit(key, 0))
				key = "_" + key;
			#region indexingkey;
			var indexkey = key;
			var i = 0;
			while (localization.ContainsKey(key) && localization[key] != text)
			{
				key = indexkey + ++i;
			}
			#endregion
			localization[key] = text.Replace("\t","    ");

			return string.Format("Resource, Resources.{0}.{1}", _filename, key);
		}

		/// <summary>
		/// Save resource file
		/// </summary>
		public void Dispose()
		{
			if (!Directory.Exists(Path.GetDirectoryName(resoursePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(resoursePath));
			}

			var asseembly = Assembly.LoadWithPartialName("System.Windows.Forms");
			var resXResourceWriterType = asseembly.GetType("System.Resources.ResXResourceWriter");
			var addResourceMethod = resXResourceWriterType.GetMethod("AddResource", new[] { typeof(string), typeof(string) });
			var closeMethod = resXResourceWriterType.GetMethod("Close");

			var resXResourceWriter = Activator.CreateInstance(resXResourceWriterType, new object[] { resoursePath });

			foreach (var item in localization)
			{
				addResourceMethod.Invoke(resXResourceWriter, new[] { item.Key, item.Value });
			}
			closeMethod.Invoke(resXResourceWriter, null);
		}
	}

	public class Backuper : IDisposable
	{
		private static object _lock = new object();
		private ZipOutputStream _zipStream = null;
		private string _zipPath = string.Empty;

		private ZipOutputStream s
		{
			get
			{
				lock (_lock)
				{
					if (_zipStream == null)
					{
						var index = 0;
						if (File.Exists(_zipPath))
						{
							var path = Path.GetDirectoryName(_zipPath);
							var name = Path.GetFileNameWithoutExtension(_zipPath);
							var ext = Path.GetExtension(_zipPath);
							do
							{
								_zipPath = Path.Combine(path, name + (++index) + ext);
							} while (File.Exists(_zipPath));
						}

						if (!Directory.Exists(Path.GetDirectoryName(_zipPath)))
							Directory.CreateDirectory(Path.GetDirectoryName(_zipPath));
						_zipStream = new ZipOutputStream(File.Create(_zipPath));
						_zipStream.SetLevel(1); // 0 - store only to 9 - means best compression

					}
				}
				return _zipStream;
			}
		}

		private byte[] buffer = new byte[4096];

		[ThreadStatic]
		private static Backuper _current = null;

		public static Backuper Current
		{
			get
			{
				return _current;
			}
		}

		public Backuper(string relativePath)
		{
			_zipPath = Path.Combine(PathUtil.BaseDirectory, relativePath);
			if (_current == null)
				_current = this;
		}

		public void Add(string file)
		{
			ZipEntry entry = new ZipEntry(file.Replace(PathUtil.BaseDirectory, "").Replace("\\", "/"));
			entry.DateTime = DateTime.Now;
			s.PutNextEntry(entry);

			using (FileStream fs = File.OpenRead(file))
			{
				int sourceBytes;
				do
				{
					sourceBytes = fs.Read(buffer, 0, buffer.Length);
					s.Write(buffer, 0, sourceBytes);
				} while (sourceBytes > 0);
			}
		}

		public void Dispose()
		{
			s.Finish();
			s.Close();
			if (_current == this)
				_current = null;
		}
	}

	public class ErrorSummary : IValidator
	{
		string _message;

		public ErrorSummary(string message)
		{
			_message = message;
		}

		public string ErrorMessage
		{
			get { return _message; }
			set { }
		}

		public bool IsValid
		{
			get { return false; }
			set { }
		}

		public void Validate()
		{ }
	}

}