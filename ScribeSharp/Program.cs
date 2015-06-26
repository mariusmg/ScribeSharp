using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using CommandLine;

namespace voidsoft.ScribeSharp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("ScribeSharp (c) 2012-2016 Marius Gheorghe");
                Console.WriteLine("Usage sample : ");
                Console.WriteLine(@"ss -i 'c:\input path' -o 'c:\output path' ");
                Console.WriteLine(@"Make sure the paths don't end with \ . Also files in output path WILL BE OVERWRITTEN !!");
                return;
            }

            CommandLineOptions options = new CommandLineOptions();

            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));

            if (!parser.ParseArguments(args, options))
            {
                Console.WriteLine("Run ss.exe to see usage description");
                Console.WriteLine("Output path parameter is required. Exiting...");
                Environment.Exit(1);
            }

	        options.InputPath = options.InputPath.Replace("'", "");
	        options.OutputPath = options.OutputPath.Replace("'", "");


            if (!options.InputPath.EndsWith(@"\"))
            {
                options.InputPath += @"\";
            }

            if (!options.OutputPath.EndsWith(@"\"))
            {
                options.OutputPath += @"\";
            }


            //check paths
            if(!Directory.Exists(options.InputPath))
            {
                Console.WriteLine("Input path is invalid");
                return;
            }


            if (!Directory.Exists(options.OutputPath))
            {
	            Directory.CreateDirectory(options.OutputPath);
            }

#if DEBUG
			Console.WriteLine("input : " + options.InputPath );
			Console.WriteLine("output : " + options.OutputPath );
#endif
			ApplicationContext.IgnoredFiles = new List<string>();
			ApplicationContext.IgnoredFolders = new List<string>();

	        string value = (string) (new AppSettingsReader()).GetValue("IgnoreList", typeof (string));


	        if (!string.IsNullOrEmpty(value))
	        {
		        string[] strings = value.Split('|');

		        foreach (string s in strings)
		        {
			        if (s.StartsWith("*"))
			        {
				        ApplicationContext.IgnoredFiles.Add(s.Substring(s.IndexOf(".")).ToLower());
			        }
			        else
			        {
						ApplicationContext.IgnoredFolders.Add(s.ToLower());
			        }
		        }
	        }

	        try
            {
                (new PathClone()).Process(options);
                (new HtmlGenerator()).Process(options);
                (new RedirectsGenerator()).Process(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Done");
        }
    }


    public class CommandLineOptions : CommandLineOptionsBase
    {
        [Option("i", "input", Required = true, HelpText = "Input path to process.")]
        public string InputPath = String.Empty;

        [Option("o", "output", Required = true, HelpText = "Output path to write processed files")]
        public string OutputPath = String.Empty;

        //[Option("v", "overwrite", Required = false, HelpText = "Overwrite outputPath")]
        //public string Overwrite = String.Empty;

    }
}