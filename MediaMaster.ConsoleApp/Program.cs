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

            MediaFile[] vboxFiles = files.Where(x => MediaFile.ParseFileOrigin(x) == FileOrigin.Vbox7).Select(x => new VboxFile(x)).ToArray();

            MediaFileDownloader downloader = new MediaFileDownloader();

            downloader.MediaFileDownloadStarting += downloader_MediaFileDownloadStarting;
            downloader.MediaFileDownloadProgress += downloader_MediaFileDownloadProgress;
            downloader.MediaFileDownloadFinished += downloader_MediaFileDownloadFinished;

            downloader.MediaFileConversionStarting += downloader_MediaFileConversionStarting;
            downloader.MediaFileConvertionCompelete += downloader_MediaFileConvertionCompelete;

            string desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DownloadedFiles");
            Directory.CreateDirectory(desktop);
            var results = downloader.DownloadAndConvert(vboxFiles, desktop, SupportedConversionFormats.Mp3, false);

            var notDownloaded = new List<DownloadResult>();
            var notConverted = new List<ConvertResult>();

            foreach (var item in results)
            {
                if (!item.DownloadResult.IsDownloaded)
                {
                    notDownloaded.Add(item.DownloadResult);
                }

                if (!item.ConversionResult.IsConverted)
                {
                    notConverted.Add(item.ConversionResult);
                }
            }

        }

        static void downloader_MediaFileConvertionCompelete(object sender, MediaFileConversionEventArgs e)
        {
            Console.WriteLine("Converted file {0}", e.MediaFile.GetMetadata().FileName);
        }

        static void downloader_MediaFileConversionStarting(object sender, MediaFileConversionEventArgs e)
        {
            Console.WriteLine("Converting file {0}", e.MediaFile.GetMetadata().FileName);
        }

        static void downloader_MediaFileDownloadFinished(object sender, MediaFileDownloadFinishedEvenArgs e)
        {
            Console.WriteLine("Finished Downloading file {0}", e.MediaFile.GetMetadata().FileName);
        }

        static void downloader_MediaFileDownloadProgress(object sender, MediaFileDownloadProgressEventArgs e)
        {
            //Console.WriteLine("Download Progress of file {0} - {1}", e.MediaFile.GetMetadata().FileName, e.PercentageComplete);
        }

        static void downloader_MediaFileDownloadStarting(object sender, MediaFileDownloadStartingEventArgs e)
        {
            Console.WriteLine("Downloading {0}", e.MediaFile.GetMetadata().FileName);
        }
    }
}
