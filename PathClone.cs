namespace ScribeSharp;

public class PathClone : IProcess
{
    public void Process(CommandLineOptions options)
    {
        //check if the output path exists
        if (!Directory.Exists(options.OutputPath)) Directory.CreateDirectory(options.OutputPath);

        CloneFolderStructure(options.InputPath, options.OutputPath);
    }

    private bool ShouldSkipFile(string fileExtension)
    {
        return ApplicationContext.IgnoredFiles.Contains(fileExtension.ToLower());
    }

    private bool ShouldSkipFolder(string name)
    {
        return ApplicationContext.IgnoredFolders.Contains(name.ToLower());
    }

    private void CloneFolderStructure(string startPath, string outputPath)
    {
        var files = Directory.GetFiles(startPath, "*.*");


        //skip htm & html files

        foreach (var file in files)
        {
            Thread.Sleep(0);

            var extension = Path.GetExtension(file).ToLower();

            var fileName = Path.GetFileName(file);

            //string lower = fileName.ToLower();


            if (ShouldSkipFile(extension)) continue;

            if (extension == ".master") continue;

            //also skip redirects.txt file
            if (fileName == "redirects.txt") continue;

            File.Copy(file, outputPath + fileName, true);
        }

        var directories = Directory.GetDirectories(startPath);

        foreach (var d in directories)
        {
            var directoryName = Path.GetFileName(d);

            if (ShouldSkipFolder(directoryName)) continue;

            Directory.CreateDirectory(outputPath + directoryName);

            CloneFolderStructure(d, outputPath + directoryName + Path.DirectorySeparatorChar);
        }
    }
}