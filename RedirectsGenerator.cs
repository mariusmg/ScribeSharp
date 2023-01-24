namespace ScribeSharp;

public class RedirectsGenerator : IProcess
{
    private const string REDIRECT_CODE =
        "Link changed.Click <a href='XXX'/>here</a> if the browser doesn't redirect you automatically.<script>window.location=XXX</script>";


    public void Process()
    {
        if (!File.Exists(ApplicationContext.InputPath + @"\redirects.txt")) return;

        var lines = File.ReadAllLines(ApplicationContext.InputPath + @"\redirects.txt");


        if (lines.Length == 0) return;

        Console.WriteLine("Started generating redirects");

        foreach (var line in lines)
            try
            {
                var parts = line.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2) continue;

                var fileStream = File.Create(ApplicationContext.OutputPath + @"\" + parts[1]);

                var writer = new StreamWriter(fileStream);

                writer.Write(REDIRECT_CODE.Replace("XXX", parts[0]));

                writer.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to generate redirect file for " + line);
            }
    }
}