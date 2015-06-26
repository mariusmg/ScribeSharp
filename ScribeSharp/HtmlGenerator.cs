using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace voidsoft.ScribeSharp
{
	public class HtmlGenerator : IProcess
	{
		public const string MASTER_PAGE_PREFIX_IDENTIFIER = "@@";

		public const string CONTENT_IDENTIFIER = "@@CONTENT";

		private Dictionary<string, string> masterPages = new Dictionary<string, string>();


		public void Process(CommandLineOptions o)
		{
			LoadMasterPages(o.InputPath);

			string[] filestoProcess = LoadFilestoProcess(o.InputPath);

			if (filestoProcess.Length == 0)
			{
				Console.WriteLine("No html files found to process");
				return;
			}

			ProcessFiles(filestoProcess, o);
		}


		private string[] GetFiles(string sourceFolder, string filters, System.IO.SearchOption searchOption)
		{
			return filters.Split('|').SelectMany(filter => System.IO.Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
		}

		private string[] LoadFilestoProcess(string path)
		{
			string[] htmFiles = GetFiles(path, "*.htm|*.html", SearchOption.TopDirectoryOnly);

			if (htmFiles.Length == 0)
			{
				Console.WriteLine("No files to process. Aborting");
			}

			return htmFiles;
		}


		private void ProcessFiles(IEnumerable<string> filePathsToBeProcessed, CommandLineOptions options)
		{
			foreach (string filePath in filePathsToBeProcessed)
			{
				try
				{
					ProcessFile(filePath, options);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					Console.WriteLine(string.Format("File {0} could not been processed ", filePath));
				}
			}
		}

		private void ProcessFile(string filePath, CommandLineOptions options)
		{
			Console.WriteLine(string.Format("Started processing file {0} ", filePath));

			FileStream fsInput = null;

			FileStream fsOutput = null;

			try
			{
				fsInput = new FileStream(filePath, FileMode.Open, FileAccess.Read);

				StreamReader inputReader = new StreamReader(fsInput);

				//read only the first line where the masterpage identifier resides
				string masterPageIdentifer = inputReader.ReadLine();


				//check for master page identifier
				if (!masterPageIdentifer.StartsWith(MASTER_PAGE_PREFIX_IDENTIFIER))
				{
					Console.WriteLine(string.Format("File {0} doesn't have a masterpage identifier", filePath));
					return;
				}

				//get the mp id by striping the @@ 
				string id = masterPageIdentifer.Substring(MASTER_PAGE_PREFIX_IDENTIFIER.Length);

				//check if we have a masterpage with this name
				if (!masterPages.ContainsKey(id))
				{
					Console.WriteLine(string.Format("MasterPage with id {0} was not found ", id));
					return;
				}

				//read all the content now
				string pageContent = inputReader.ReadToEnd();

				//at this point we can close the input 
				if (fsInput != null)
				{
					fsInput.Close();
				}

				//check if the output file exists now and if we are allowed to overwrite

				//get the mp content
				string mpContent = masterPages[id];

				pageContent = mpContent.Replace(CONTENT_IDENTIFIER, pageContent);

				//get the correct output path by replace the output in input
				string outputPath = filePath.Replace(options.InputPath, options.OutputPath);

				fsOutput = new FileStream(outputPath, FileMode.Truncate, FileAccess.Write);

				StreamWriter outputWriter = new StreamWriter(fsOutput);

				outputWriter.Write(pageContent);

				outputWriter.Flush();
			}
			finally
			{
				if (fsInput != null)
				{
					fsInput.Close();
				}

				if (fsOutput != null)
				{
					fsOutput.Close();
				}
			}
		}


		private void LoadMasterPages(string path)
		{
			string[] masterPageFiles = Directory.GetFiles(path, "*.Master");

			if (masterPageFiles.Length == 0)
			{
				Console.WriteLine("No master pages detected. Aborting");
				return;
			}

			//load em up
			foreach (string page in masterPageFiles)
			{
				try
				{
					string content = File.ReadAllText(page);

					string fileName = Path.GetFileNameWithoutExtension(page);

					masterPages.Add(fileName, content);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Can' read " + page);
					return;
				}
			}

			Console.WriteLine("Loaded master pages");
		}
	}
}