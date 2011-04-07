using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Composite.C1Console.Security;
using Composite.Core.IO;
using ICSharpCode.SharpZipLib.Zip;

public partial class XmlBasedSiteBackup : Page
{
	string hash = "App_Data\\Backups";
	private static object _lock = new object();
	protected readonly string _deleteCommand = "DeleteCommand";
	public Repeater BackupList;

	public string BackupFilename
	{
		get
		{
			return "backup";
		}
	}

	public string BaseDirectory
	{
		get
		{
			return PathUtil.BaseDirectory;
		}

	}
	public string BackupDirectory
	{
		get
		{
			return PathCombine(PathUtil.BaseDirectory, hash, "XmlBasedSiteBackup");
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!UserValidationFacade.IsLoggedIn())
		{
			Response.Redirect(string.Format("Composite/Login.aspx?ReturnUrl={0}", HttpUtility.HtmlEncode(Request.Url.PathAndQuery)));
		}

		var backupFile = Request[BackupFilename];
		if (backupFile != null)
		{
			TransmitBackup(Path.Combine(BackupDirectory, Path.GetFileName(backupFile)));
		}

	}

	protected void Page_PreRender(object sender, EventArgs e)
	{
		if (IsPostBack && Validators.Count == 0)
		{
			Response.Redirect(Request.Url.ToString());
		}
		try
		{
			BackupList.DataSource = from f in C1Directory.GetFiles(BackupDirectory, "*.zip")
									orderby f
									select new
									{
										Filename = Path.GetFileName(f),
										DateCreated = C1File.GetCreationTime(f),
										Filepath = string.Format("?{0}={1}", BackupFilename, Path.GetFileName(f)),
										Filesize = ((int)((new C1FileInfo(f).Length) / 1000)).ToString("N0")
									};
			BackupList.DataBind();
		}
		catch
		{
		}

	}

	protected void TransmitBackup(string filePath)
	{
		Response.Clear();
		Response.ClearContent();
		Response.ClearHeaders();

		Response.ContentType = "application/zip";
		Response.AppendHeader("Content-Disposition", string.Format(@"attachment; filename=""{0}""", Path.GetFileName(filePath)));
		Response.AddHeader("Content-Length", new C1FileInfo(filePath).Length.ToString());
		Response.TransmitFile(filePath);
		Response.End();
	}

	protected string CreateBackup()
	{
		var Url = Request.Url;
		string zipFilename = Url.Scheme + "_" + Url.Host + (Url.IsDefaultPort ? "" : (" " + Url.Port)) + "_" + DateTime.Now.ToString("yyyy_MM.dd_HHmm") + ".zip";
		string zipFilePath = PathCombine(BackupDirectory, zipFilename);
		if (!C1Directory.Exists(Path.GetDirectoryName(zipFilePath)))
			C1Directory.CreateDirectory(Path.GetDirectoryName(zipFilePath));

		#region Zipping

		var filenames = GetFilesRecursive(BaseDirectory);
		var directories = GetDirectoriesRecursive(BaseDirectory);

		using (ZipOutputStream s = new ZipOutputStream(FileCreate(zipFilePath)))
		{
			lock (_lock)
			{
				int oldScriptTimeOut = Server.ScriptTimeout;
				Server.ScriptTimeout = 600; // 10 minutes

				s.SetLevel(1); // 0 - store only to 9 - means best compression
				byte[] buffer = new byte[4096];

				foreach (string directory in directories)
				{
					if (directory.Contains(hash))
						continue;
					ZipEntry entry = new ZipEntry(directory.Replace(BaseDirectory, "").Replace("\\", "/") + "/");
					entry.DateTime = DateTime.Now;
					s.PutNextEntry(entry);
					s.CloseEntry();
				}

				foreach (string file in filenames)
				{
					if (file.Contains(hash))
						continue;
					ZipEntry entry = new ZipEntry(file.Replace(BaseDirectory, "").Replace("\\", "/"));
					var fi = new C1FileInfo(file);
					entry.DateTime = fi.LastWriteTime;
					entry.Size = fi.Length;

					try
					{
						using (C1FileStream fs = C1File.OpenRead(file))
						{
							s.PutNextEntry(entry);
							int sourceBytes;
							do
							{
								sourceBytes = fs.Read(buffer, 0, buffer.Length);
								s.Write(buffer, 0, sourceBytes);
							} while (sourceBytes > 0);
						}
					}
					catch (Exception e)
					{
						if (!file.Contains("App_Data\\Composite\\LogFiles"))
						{
							this.Validators.Add(new ErrorSummary(e.Message));
						}
					}
				}

				Server.ScriptTimeout = oldScriptTimeOut;
			}
			s.Finish();
			s.Close();
		}
		#endregion
		return zipFilename;
	}



	private static C1FileStream FileCreate(string filePath)
	{
		lock (_lock)
		{
			var index = 0;
			if (C1File.Exists(filePath))
			{
				var path = Path.GetDirectoryName(filePath);
				var name = Path.GetFileNameWithoutExtension(filePath);
				var ext = Path.GetExtension(filePath);
				do
				{
					filePath = Path.Combine(path, name + "." + (++index) + ext);
				} while (C1File.Exists(filePath));
			}

			return C1File.Create(filePath);
		}
	}


	protected void CreateBackup_Click(object sender, EventArgs e)
	{
		CreateBackup();
	}

	public static string PathCombine(params string[] path)
	{
		if (path.Length == 0)
			return string.Empty;
		return path.Aggregate((path1, path2) => Path.Combine(path1, path2));
	}

	public List<string> GetFilesRecursive(string b)
	{
		List<string> result = new List<string>();
		Stack<string> stack = new Stack<string>();
		stack.Push(b);
		while (stack.Count > 0)
		{
			string dir = stack.Pop();
			try
			{
				result.AddRange(C1Directory.GetFiles(dir, "*.*"));
				foreach (string dn in C1Directory.GetDirectories(dir))
				{
					stack.Push(dn);
				}
			}
			catch (Exception e)
			{
				this.Validators.Add(new ErrorSummary(e.Message));
			}
		}
		return result;
	}

	public List<string> GetDirectoriesRecursive(string b)
	{
		List<string> result = new List<string>();
		Stack<string> stack = new Stack<string>();
		stack.Push(b);
		while (stack.Count > 0)
		{
			string dir = stack.Pop();
			try
			{
				result.AddRange(C1Directory.GetDirectories(dir, "*.*"));
				foreach (string dn in C1Directory.GetDirectories(dir))
				{
					stack.Push(dn);
				}
			}
			catch (Exception e)
			{
				this.Validators.Add(new ErrorSummary(e.Message));
			}
		}
		return result;
	}

	protected void BackupList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
	{
		if (e.CommandName == _deleteCommand)
		{
			try
			{
				C1File.Delete(Path.Combine(BackupDirectory, e.CommandArgument.ToString()));
			}
			catch (Exception ex)
			{
				this.Validators.Add(new ErrorSummary(ex.Message));
			}
		}
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
