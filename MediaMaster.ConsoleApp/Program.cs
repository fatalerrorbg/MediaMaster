using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> files = new List<string>();

            string line = null;
            while ((line = Console.ReadLine()) != "")
            {
                files.Add(line);
            }

            WebFile[] vboxFiles = files.Where(x => WebFile.ParseFileOrigin(x) == FileOrigin.Vbox7).Select(x => new VboxFile(x)).ToArray();

            WebFileDownloader downloader = new WebFileDownloader();

            downloader.WebFileDownloadStarting += downloader_WebFileDownloadStarting;
            downloader.WebFileDownloadProgress += downloader_WebFileDownloadProgress;
            downloader.WebFileDownloadFinished += downloader_WebFileDownloadFinished;

            downloader.WebFileConversionStarting += downloader_WebFileConversionStarting;
            downloader.WebFileConvertionCompelete += downloader_WebFileConvertionCompelete;

            string desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DownloadedFiles");
            Directory.CreateDirectory(desktop);
            downloader.DownloadAndConvert(vboxFiles, desktop, SupportedConversionFormats.Mp3, false);

        }

        static void downloader_WebFileConvertionCompelete(object sender, WebFileConversionEventArgs e)
        {
            Console.WriteLine("Converted file {0}", e.WebFile.GetMetadata().FileName);
        }

        static void downloader_WebFileConversionStarting(object sender, WebFileConversionEventArgs e)
        {
            Console.WriteLine("Converting file {0}", e.WebFile.GetMetadata().FileName);
        }

        static void downloader_WebFileDownloadFinished(object sender, WebFileDownloadFinishedEvenArgs e)
        {
            Console.WriteLine("Finished Downloading file {0}", e.WebFile.GetMetadata().FileName);
        }

        static void downloader_WebFileDownloadProgress(object sender, WebFileDownloadProgressEventArgs e)
        {
            //Console.WriteLine("Download Progress of file {0} - {1}", e.WebFile.GetMetadata().FileName, e.PercentageComplete);
        }

        static void downloader_WebFileDownloadStarting(object sender, WebFileDownloadStartingEventArgs e)
        {
            Console.WriteLine("Downloading {0}", e.WebFile.GetMetadata().FileName);
        }
    }
}
