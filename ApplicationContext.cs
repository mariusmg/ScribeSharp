namespace ScribeSharp;

public class ApplicationContext
{
    public static List<string> IgnoredFolders { get; set; } 

    public static List<string> IgnoredFiles { get; set; }


    public static string InputPath
    {
        get;
        set;
    }

    public static string OutputPath
    {
        get;
        set;
    }
}