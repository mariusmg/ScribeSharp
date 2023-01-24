using Microsoft.Extensions.Configuration;

namespace ScribeSharp;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0 || args.Length != 2)
        {
            Console.WriteLine("ScribeSharp (c) Marius Gheorghe");
            Console.WriteLine("Usage sample : ");
            Console.WriteLine(@"ScribeSharp.exe 'c:\input path' 'c:\output path'");
            Console.WriteLine(@"Make sure the paths don't end with \ . Also files in output path WILL BE OVERWRITTEN !!");
            return;
        }

        // CommandLineOptions options = new CommandLineOptions();
        // ParserResult<CommandLineOptions>? parsed = Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(o => { });


        if (string.IsNullOrEmpty(args[0]))
        {
            Console.WriteLine("Run ss.exe to see usage description");
            Console.WriteLine("Input path parameter is required. Exiting...");
            Environment.Exit(1);
        }
        
        if (string.IsNullOrEmpty(args[1]))
        {
            Console.WriteLine("Run ss.exe to see usage description");
            Console.WriteLine("Output path parameter is required. Exiting...");
            Environment.Exit(1);
        }

        ApplicationContext.InputPath = args[0];
        ApplicationContext.OutputPath = args[1];

        //check paths
        if (!Directory.Exists(ApplicationContext.InputPath))
        {
            Console.WriteLine($"Input path '{ApplicationContext.InputPath}' is invalid");
            return;
        }

        if (!Directory.Exists(ApplicationContext.OutputPath))
        {
            Directory.CreateDirectory(ApplicationContext.OutputPath);
        }

        ApplicationContext.IgnoredFiles = new List<string>();
        ApplicationContext.IgnoredFolders = new List<string>();


        IConfigurationBuilder configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json");

        IConfigurationRoot config = configuration.Build();
        string? value = config.GetSection("Settings")["IgnoreList"];


        if (!string.IsNullOrEmpty(value))
        {
            string[] strings = value.Split('|');

            foreach (string s in strings)
                if (s.StartsWith("*"))
                    ApplicationContext.IgnoredFiles.Add(s.Substring(s.IndexOf(".")).ToLower());
                else
                    ApplicationContext.IgnoredFolders.Add(s.ToLower());
        }

        
        Console.WriteLine(ApplicationContext.IgnoredFolders.Count + " folders will be ignored");
        Console.WriteLine(ApplicationContext.IgnoredFiles.Count + " file types will be ignored");
        
        try
        {
            new PathClone().Process();
            new HtmlGenerator().Process();
            new RedirectsGenerator().Process();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        Console.WriteLine("DONE");
    }
}

