using CommandLine;
using Microsoft.Extensions.Configuration;

namespace ScribeSharp;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("ScribeSharp (c) Marius Gheorghe");
            Console.WriteLine("Usage sample : ");
            Console.WriteLine(@"ss -i 'c:\input path' -o 'c:\output path' ");
            Console.WriteLine(
                @"Make sure the paths don't end with \ . Also files in output path WILL BE OVERWRITTEN !!");
            return;
        }

        var options = new CommandLineOptions();

        var parsed = Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(o => { });


        if (string.IsNullOrEmpty(parsed.Value.InputPath))
        {
            Console.WriteLine("Run ss.exe to see usage description");
            Console.WriteLine("Input path parameter is required. Exiting...");
            Environment.Exit(1);
        }

        if (string.IsNullOrEmpty(parsed.Value.OutputPath))
        {
            Console.WriteLine("Run ss.exe to see usage description");
            Console.WriteLine("Output path parameter is required. Exiting...");
            Environment.Exit(1);
        }

        options.InputPath = options.InputPath.Replace("'", "");
        options.OutputPath = options.OutputPath.Replace("'", "");

        if (!options.InputPath.EndsWith(@"\")) options.InputPath += @"\";

        if (!options.OutputPath.EndsWith(@"\")) options.OutputPath += @"\";


        //check paths
        if (!Directory.Exists(options.InputPath))
        {
            Console.WriteLine($"Input path '{options.InputPath}' is invalid");
            return;
        }


        if (!Directory.Exists(options.OutputPath)) Directory.CreateDirectory(options.OutputPath);

#if DEBUG
        Console.WriteLine("input : " + options.InputPath);
        Console.WriteLine("output : " + options.OutputPath);
#endif
        ApplicationContext.IgnoredFiles = new List<string>();
        ApplicationContext.IgnoredFolders = new List<string>();


        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");

        var config = configuration.Build();
        var value = config.GetConnectionString("IgnoreList");


        if (!string.IsNullOrEmpty(value))
        {
            var strings = value.Split('|');

            foreach (var s in strings)
                if (s.StartsWith("*"))
                    ApplicationContext.IgnoredFiles.Add(s.Substring(s.IndexOf(".")).ToLower());
                else
                    ApplicationContext.IgnoredFolders.Add(s.ToLower());
        }

        try
        {
            new PathClone().Process(options);
            new HtmlGenerator().Process(options);
            new RedirectsGenerator().Process(options);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        Console.WriteLine("DONE");
    }
}

public class CommandLineOptions
{
    [Option('i', "input", Required = true, HelpText = "Input path to process.")]
    public string InputPath { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output path to write processed files")]
    public string OutputPath { get; set; }
}