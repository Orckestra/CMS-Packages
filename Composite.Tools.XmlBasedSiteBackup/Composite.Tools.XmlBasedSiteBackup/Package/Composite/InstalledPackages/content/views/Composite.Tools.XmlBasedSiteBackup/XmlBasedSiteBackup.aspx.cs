using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.UI;
using Composite.C1Console.Security;
using Composite.Core.IO;

public partial class XmlBasedSiteBackup : Page
{
    private static readonly CompressionLevel BackupCompressionLevel = CompressionLevel.Fastest;

    const string BackupDirectoryRelativePath = "App_Data\\Backups";
    private const int ScriptTimeoutInSeconds = 600;  // 10 minutes

    readonly string[] TemporaryDirectories = 
        {
            "\\App_Data\\Composite\\Cache\\Assemblies",
            "\\App_Data\\Composite\\Cache\\Resized images", 
            "\\App_Data\\Composite\\Cache\\ResourceCache",
            "\\App_Data\\Composite\\ApplicationState\\LockFiles",
            "\\App_Data\\Composite\\ApplicationState\\SerializedConsoleMessages"
        };

    private static readonly object _lock = new object();

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
            return PathCombine(PathUtil.BaseDirectory, BackupDirectoryRelativePath, "XmlBasedSiteBackup");
        }
    }

    protected string GetConfirmationCode(object fileName)
    {
        return "confirmDeleteXMLBasedBackup('" + (string)fileName + "')";
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

        if (Page.IsPostBack)
        {
            string commandName = Request["commandName"];
            if (commandName == "delete")
            {
                string deleteXMLBackupFile = Request["deleteXMLBackupFile"];
                if (!string.IsNullOrWhiteSpace(deleteXMLBackupFile))
                {
                    try
                    {
                        C1File.Delete(Path.Combine(BackupDirectory, deleteXMLBackupFile));
                    }
                    catch (Exception ex)
                    {
                        this.Validators.Add(new ErrorSummary(ex.Message));
                    }
                }
            }

            if (commandName == "create")
            {
                CreateBackup();
            }
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
        string zipFilename = Url.Scheme + "_" + Url.Host + (Url.IsDefaultPort ? "" : (" " + Url.Port)) + "_" +
                             DateTime.Now.ToString("yyyy_MM.dd_HHmm") + ".zip";
        string zipFilePath = PathCombine(BackupDirectory, zipFilename);

        string backupDirectory = Path.GetDirectoryName(zipFilePath);
        if (!C1Directory.Exists(backupDirectory))
        {
            C1Directory.CreateDirectory(backupDirectory);
        }
            

        var filenames = GetFilesRecursive(BaseDirectory);
        var directories = GetDirectoriesRecursive(BaseDirectory);

        lock (_lock)
        {
            using(var archiveFileStream = FileCreate(zipFilePath))
            using (var archive = new ZipArchive(archiveFileStream, ZipArchiveMode.Create))
            {
                int oldScriptTimeOut = Server.ScriptTimeout;
                Server.ScriptTimeout = ScriptTimeoutInSeconds;

                foreach (string directory in directories)
                {
                    if (directory.Contains(BackupDirectoryRelativePath))
                        continue;

                    string directoryEntryName = directory.Replace(BaseDirectory, "").Replace("\\", "/") + "/";

                    var dirEntry = archive.CreateEntry(directoryEntryName);
                    dirEntry.LastWriteTime = Directory.GetLastWriteTime(directory);
                }

                foreach (string file in filenames)
                {
                    if (file.Contains(BackupDirectoryRelativePath) || IsTemporaryDirectory(file))
                    {
                        continue;
                    }

                    string fileEntryName = file.Replace(BaseDirectory, "").Replace("\\", "/");

                    var entry = archive.CreateEntry(fileEntryName, BackupCompressionLevel);

                    try
                    {
                        var fi = new C1FileInfo(file);
                        entry.LastWriteTime = fi.LastWriteTime;

                        using (var fileStream = C1File.OpenRead(file))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
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
        }
  
        return zipFilename;
    }

    private static C1FileStream FileCreate(string filePath)
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


    protected void CreateBackup_Click(object sender, EventArgs e)
    {
        CreateBackup();
    }

    public static string PathCombine(params string[] path)
    {
        if (path.Length == 0)
            return string.Empty;
        return path.Aggregate(Path.Combine);
    }

    public List<string> GetFilesRecursive(string rootDirectory)
    {
        List<string> result = new List<string>();
        Stack<string> stack = new Stack<string>();
        stack.Push(rootDirectory);
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

    public List<string> GetDirectoriesRecursive(string rootDirectory)
    {
        List<string> result = new List<string>();
        Stack<string> stack = new Stack<string>();
        stack.Push(rootDirectory);
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

    private bool IsTemporaryDirectory(string fileOrDirectoryPath)
    {
        return TemporaryDirectories.Any(tempDir => fileOrDirectoryPath.IndexOf(tempDir, StringComparison.OrdinalIgnoreCase) > -1);
    }

}

public class ErrorSummary : IValidator
{
    readonly string _message;

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
