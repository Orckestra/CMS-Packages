using System.Diagnostics;

namespace C1PackageDevProvisioner.PackageServer
{
    public static class PackageServerProcessManager
    {
        private static Process _iisExpressProcess;

        public static void Start()
        {
            _iisExpressProcess = new Process();

            _iisExpressProcess.StartInfo.FileName = Configration.IISExpressPath;
            _iisExpressProcess.StartInfo.Arguments = string.Format("/systray:false /path:\"{0}\" /port:{1}",
                FileUtil.MapIfRelative(Configration.MockPackageServerPath),
                Configration.MockPackageServerPort
                );

            // systray:false /path:xxx /port:888

            _iisExpressProcess.StartInfo.CreateNoWindow = true;
            _iisExpressProcess.StartInfo.UseShellExecute = false;
            _iisExpressProcess.StartInfo.RedirectStandardInput = false;
            _iisExpressProcess.StartInfo.RedirectStandardOutput = false;

            _iisExpressProcess.Start();
        }

        public static void Stop()
        {
            if (!_iisExpressProcess.HasExited)
            {
                _iisExpressProcess.Kill();
                _iisExpressProcess.WaitForExit(500);
            }
        }
    }
}
