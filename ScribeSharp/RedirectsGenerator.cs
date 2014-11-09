using System;
using System.IO;

namespace voidsoft.ScribeSharp
{
    public class RedirectsGenerator : IProcess
    {

        private const string REDIRECT_CODE = "Link changed.Click <a href='XXX'/>here</a> if the browser doesn't redirect you automatically.<script>window.location=XXX</script>";


        private void GenerateRedirects(CommandLineOptions options)
        {
            if (!File.Exists(options.InputPath + @"\redirects.txt"))
            {
                return;
            }

            string[] lines = File.ReadAllLines(options.InputPath + @"\redirects.txt");


            if (lines.Length == 0)
            {
                return;
            }

            Console.WriteLine("Started generating redirects");

            foreach (string line in lines)
            {
                try
                {
                    string[] parts = line.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 2)
                    {
                        continue;
                    }

                    FileStream fileStream = File.Create(options.OutputPath + @"\" + parts[1]);

                    StreamWriter writer = new StreamWriter(fileStream);

                    writer.Write(REDIRECT_CODE.Replace("XXX", parts[0]));

                    writer.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to generate redirect file for " + line);
                }
            }
        }



        public void Process(CommandLineOptions options)
        {
            GenerateRedirects(options);
        }
    }
}