using System.IO;
using System.Threading;

namespace voidsoft.ScribeSharp
{
	public class PathClone : IProcess
	{
		public void Process(CommandLineOptions options)
		{
			//check if the output path exists
			if (!Directory.Exists(options.OutputPath))
			{
				Directory.CreateDirectory(options.OutputPath);
			}

			CloneFolderStructure(options.InputPath, options.OutputPath);
		}

		private void CloneFolderStructure(string startPath, string outputPath)
		{
			string[] files = Directory.GetFiles(startPath,"*.*");


			//skip htm & html files

			foreach (string file in files)
			{
				Thread.Sleep(0);

				string extension = Path.GetExtension(file).ToLower();

				string fileName = Path.GetFileName(file);

				string lower = fileName.ToLower();

				if (lower == "build.ps1" || lower == "build.bat" || lower == "commandline.dll" || lower == "ss.exe")
				{
					continue;
				}

				//skip files *.master & redirect

				//if (extension == ".html" || extension == ".htm" || extension == ".master")

				if (extension == ".master")
				{
					continue;
				}

				//also skip redirects.txt file
				if (fileName == "redirects.txt")
				{
					continue;
				}

				File.Copy(file, outputPath + fileName, true);
			}

			string[] directories = Directory.GetDirectories(startPath);

			foreach (string d in directories)
			{
				string directoryName = Path.GetFileName(d);

				Directory.CreateDirectory(outputPath + directoryName);

				CloneFolderStructure(d, outputPath + directoryName + Path.DirectorySeparatorChar);
			}
		}
	}
}