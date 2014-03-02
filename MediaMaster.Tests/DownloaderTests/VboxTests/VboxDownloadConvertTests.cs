using System;
using System.IO;
using MediaMaster.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaMaster.Tests.DownloaderTests.VboxTests
{
    [TestClass]
    public class VboxDownloadConvertTests
    {
        public const string VboxDownloadVideo = "http://www.vbox7.com/play:86c35f6759";
        public string VboxDownloadedVideoPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Зайчета атакуват момиче - Vbox7.com" + SupportedConversionFormats.Mp4);

        [TestMethod]
        public void DownloadFileTest()
        {
            MediaDownloader downloader = new MediaDownloader();
            MediaFile file = MediaFile.CreateNew(VboxDownloadVideo);
            string downloadedFile = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).DownloadPath;

            Assert.AreEqual(VboxDownloadedVideoPath, downloadedFile);
        }

        [TestMethod]
        public void DownloadFileStartingEventTest()
        {
            MediaDownloader downloader = new MediaDownloader();
            bool fired = false;
            MediaFile file = MediaFile.CreateNew(VboxDownloadVideo);
            downloader.MediaFileDownloadStarting += (s, e) => fired = true;
            string downloadedFile = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).DownloadPath;

            Assert.AreEqual(fired, true);
        }

        [TestMethod]
        public void DownloadFileProgressEventTest()
        {
            MediaDownloader downloader = new MediaDownloader();

            bool hasDownloadSize = false;
            bool hasMaxSize = false;
            bool hasProgress = false;

            MediaFile file = MediaFile.CreateNew(VboxDownloadVideo);
            downloader.MediaFileDownloadProgress += (s, e) =>
                {
                    hasDownloadSize = e.DownloadedSize > 0;
                    hasMaxSize = e.FileSize > 0;
                    hasProgress = e.PercentageComplete > 0;
                };

            string downloadedFile = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).DownloadPath;

            Assert.AreEqual(true, hasDownloadSize && hasMaxSize && hasProgress);
        }

        [TestMethod]
        public void DownloadFileCompletedEventTest()
        {
            MediaDownloader downloader = new MediaDownloader();

            bool downloaded = false;

            MediaFile file = MediaFile.CreateNew(VboxDownloadVideo);
            downloader.MediaFileDownloadFinished += (s, e) =>
            {
                downloaded = true;
            };

            string downloadedFile = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).DownloadPath;

            Assert.AreEqual(true, downloaded);
        }

        [TestMethod]
        public void ConversionStartingEventTest()
        {
            MediaDownloader downloader = new MediaDownloader();

            MediaFile file = MediaFile.CreateNew(VboxDownloadVideo);
            string downloadedPath = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).DownloadPath;
            bool converting = false;

            MediaConverter converter = new MediaConverter();
            converter.MediaFileConversionStarting += delegate { converting = true; };

            converter.Convert(file, downloadedPath, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), new MediaConverterMetadata
            {
                AudioBitrate = Bitrates.Kbps192,
                Extension = SupportedConversionFormats.Mp3,
                FileName = file.GetMetadata().FileName
            });

            Assert.AreEqual(true, converting);
        }

        [TestMethod]
        public void ConversionEndedEventTest()
        {
            MediaDownloader downloader = new MediaDownloader();

            MediaFile file = MediaFile.CreateNew(VboxDownloadVideo);
            string downloadedPath = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).DownloadPath;
            bool converting = false;

            MediaConverter converter = new MediaConverter();

            converter.MediaFileConvertionCompelete += delegate { converting = true; };
            converter.Convert(file, downloadedPath, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), new MediaConverterMetadata
            {
                AudioBitrate = Bitrates.Kbps192,
                Extension = SupportedConversionFormats.Mp3,
                FileName = file.GetMetadata().FileName
            });

            Assert.AreEqual(true, converting);
        }
    }
}
