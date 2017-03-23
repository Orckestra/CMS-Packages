using System;
using System.IO;
using System.Text;

namespace Orckestra.Search.IFilterParser
{
    class Program
    {
        private static int Main(string[] args)
        {
            // Executable name has to be "filtdump.exe" to get access to the Adobe PDF IFilter 
            // installed with Acrobat Reader

            if (args.Length != 2)
            {
                return Test();
            }

            string inputFile = args[0];
            string outputFile = args[1];

            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"File '{inputFile}' not found");
                return 1;
            }

            int statusCode;

            using (var outputStream = File.OpenWrite(outputFile))
            using (var sw = new StreamWriter(outputStream, Encoding.UTF8))
            {
                statusCode = FilterFacade.Extract(inputFile, sw);
            }

            Console.Write(FilterFacade.GetResposeMessage(statusCode));

            return statusCode;
        }

        static int Test()
        {
            var files = Directory.GetFiles(@"C:\Work\Test\DocTypes", "*.*");

            foreach (var file in files)
            {
                string result;
                int code;

                using (var ms = new MemoryStream())
                {
                    using (var sw = new StreamWriter(ms, Encoding.UTF8, 65536, true))
                    {
                        code = FilterFacade.Extract(file, sw);
                    }

                    result = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                }

                string status = FilterFacade.GetResposeMessage(code);

                Console.WriteLine($"'{status}': {file}: {(result ?? "").Length}");
                Console.ReadLine();

                if (!string.IsNullOrEmpty(result))
                {
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine(result);
                    Console.WriteLine("--------------------------------------------");
                }
            }
            return 0;
        }
    }
}
