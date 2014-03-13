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

            MediaFile[] vboxFiles = files.Select(x => MediaFile.CreateNew(x)).Where(x => x != null).ToArray();

            MediaDownloadConvertManager manager = new MediaDownloadConvertManager();
            manager.MaxParallelRequests = 2;

            manager.Downloader.MediaFileDownloadStarting += downloader_MediaFileDownloadStarting;
            manager.Downloader.MediaFileDownloadProgress += downloader_MediaFileDownloadProgress;
            manager.Downloader.MediaFileDownloadFinished += downloader_MediaFileDownloadFinished;

            manager.Converter.MediaFileConversionStarting += downloader_MediaFileConversionStarting;
            manager.Converter.MediaFileConvertionCompelete += downloader_MediaFileConvertionCompelete;

            string desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DownloadedFiles");
            Directory.CreateDirectory(desktop);
            for (int i = 0; i < vboxFiles.Length; i++)
            {
                MediaConverterMetadata metadata = null;
                if (vboxFiles[i].FileOrigin == FileOrigin.SoundCloud)
                {
                    metadata = MediaConverterMetadata.Default;
                }
                else
                {
                    metadata = new MediaConverterMetadata(Converter.Bitrates.Kbps192, vboxFiles[i].Metadata.FileName, SupportedConversionFormat.Mp3);
                }

                manager.EnqueueDownloadAndConvertRequest(vboxFiles[i], desktop, metadata);
            }

            manager.StartDownload();

            Console.ReadLine();
            Console.ReadLine();
        }

        static void downloader_MediaFileConvertionCompelete(object sender, MediaFileConversionEventArgs e)
        {
            Console.WriteLine("Converted file {0}", e.MediaFile.Metadata.FileName);
        }

        static void downloader_MediaFileConversionStarting(object sender, MediaFileConversionEventArgs e)
        {
            Console.WriteLine("Converting file {0}", e.MediaFile.Metadata.FileName);
        }

        static void downloader_MediaFileDownloadFinished(object sender, MediaDownloadFinishedEvenArgs e)
        {
            Console.WriteLine("Finished Downloading file {0}", e.MediaFile.Metadata.FileName);
        }

        static void downloader_MediaFileDownloadProgress(object sender, MediaDownloadProgressEventArgs e)
        {
            //Console.Clear();
            //Console.WriteLine("Download Progress of file {0} - {1}", e.MediaFile.Metadata.FileName, e.PercentageComplete);
        }

        static void downloader_MediaFileDownloadStarting(object sender, MediaDownloadStartingEventArgs e)
        {
            Console.WriteLine("Downloading {0}", e.MediaFile.Metadata.FileName);
        }
    }
}
