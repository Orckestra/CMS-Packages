using System;
using System.Linq;

using C1PackageDevProvisioner;
using C1PackageDevProvisioner.C1Packageserver;
using C1PackageDevProvisioner.TestSite;
using C1PackageDevProvisioner.C1Packages;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("C1 Package Development Helper");
            Console.WriteLine("______________________________________________");
            Console.WriteLine();

            var configValidationErrors = Configration.GetValidationErrors();

            if (configValidationErrors.Any())
            {
                Console.WriteLine("Configuration file is not complete - please correct these issues:");
                Console.WriteLine();

                foreach (var item in configValidationErrors)
                {
                    Console.WriteLine("{0}: {1}", item.Key, item.Value);
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
            else
            {
                bool moveOn = false;
                Console.Write("Reset C1 site to base image? [Yes / No / Quit]");
                while (!moveOn)
                {
                    var keyPress = Console.ReadKey();
                    Console.WriteLine();
                    if (Char.ToLower(keyPress.KeyChar) == 'q')
                    {
                        return;
                    }
                    if (Char.ToLower(keyPress.KeyChar) == 'n')
                    {
                        moveOn = true;
                    }
                    if (Char.ToLower(keyPress.KeyChar) == 'y')
                    {
                        Console.WriteLine("Initializing base image and mirrorring to working C1 Site - this make take a few minutes ...");
                        C1BaseImageProvider.InitializeBaseImage();
                        MirrorProcessManager.MirrorBaseToWorkingSite();
                        CompositeConfigManager.RegisterMockPackageServer();
                        moveOn = true;
                    }

                }

                Console.WriteLine();
                Console.WriteLine("Building missing / updated packages...");


                try
                {
                    PackageServerConfigManager.UpdatePackageServerConfig();
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(ex);
                    Console.ReadLine();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                WriteEventsToConsole();
                Console.WriteLine();

                PackageServerProcessManager.Start();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Package Server running at http://localhost:{0}/C1.asmx", Configration.MockPackageServerPort);
                Console.WriteLine();

                try
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Monitoring file changes in repo paths - for installed packages, will sync file updates to website.");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to exit...");

                    using (var syncManager = new FileChangeSyncManager())
                    {
                        syncManager.StartSync(true);
                        var run = true;

                        while (run)
                        {
                            WriteEventsToConsole();
                            if (Console.KeyAvailable)
                            {
                                run = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(ex);
                    Console.ReadLine();
                }
                finally
                {
                    PackageServerProcessManager.Stop();
                }

            }
        }

        private static void WriteEventsToConsole()
        {
            string eventInfo = "";
            while (EventInfo.Queue.TryDequeue(out eventInfo))
            {
                Console.WriteLine(eventInfo);
            }
        }
    }
}
