using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace C1PackageDevProvisioner.TestSite
{
    public static class MirrorProcessManager
    {
        private static Process _roboCopyProcess;

        public static void MirrorBaseToWorkingSite()
        {
            // recycle web app - this should ensure that logs writes are not lingering (and written after we synced)
            var rootWebConfigPath = Path.Combine(Configration.C1SitePath, "web.config");
            File.SetLastWriteTimeUtc(rootWebConfigPath, DateTime.UtcNow);
            Thread.Sleep(500);

            string baseImageDir = C1BaseImageProvider.GetBaseImagePath();

            string TopLevelWebCOnfigPath = Directory.GetFiles(baseImageDir, "web.config", SearchOption.AllDirectories).OrderBy(s => s.Length).First();


            string srcDir = Path.GetDirectoryName(TopLevelWebCOnfigPath);
            string destDir = Configration.C1SitePath;

            _roboCopyProcess = new Process();

            _roboCopyProcess.StartInfo.FileName = "robocopy.exe";
            _roboCopyProcess.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\" /mir", srcDir, destDir);

            _roboCopyProcess.StartInfo.CreateNoWindow = true;
            _roboCopyProcess.StartInfo.UseShellExecute = true;
            _roboCopyProcess.StartInfo.RedirectStandardInput = false;
            _roboCopyProcess.StartInfo.RedirectStandardOutput = false;

            _roboCopyProcess.Start();
            _roboCopyProcess.WaitForExit();
        }
    }
}
