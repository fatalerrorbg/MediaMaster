using System;
using System.IO;
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
            MediaFileDownloader downloader = new MediaFileDownloader();
            MediaFile file = MediaFile.CreateNew(VboxDownloadVideo);
            string downloadedFile = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).DownloadPath;

            Assert.AreEqual(VboxDownloadedVideoPath, downloadedFile);
        }

        [TestMethod]
        public void DownloadFileStartingEventTest()
        {
            MediaFileDownloader downloader = new MediaFileDownloader();
            bool fired = false;
            MediaFile file = MediaFile.CreateNew(VboxDownloadVideo);
            downloader.MediaFileDownloadStarting += (s, e) => fired = true;
            string downloadedFile = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).DownloadPath;

            Assert.AreEqual(fired, true);
        }

        [TestMethod]
        public void DownloadFileProgressEventTest()
        {
            MediaFileDownloader downloader = new MediaFileDownloader();

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
            MediaFileDownloader downloader = new MediaFileDownloader();

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
            MediaFileDownloader downloader = new MediaFileDownloader();

            MediaFile file = MediaFile.CreateNew(VboxDownloadVideo);
            string downloadedPath = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).DownloadPath;
            bool converting = false;
            downloader.MediaFileConversionStarting += delegate { converting = true; };
            downloader.ConvertSingleFile(file, downloadedPath, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), SupportedConversionFormats.Mp3);

            Assert.AreEqual(true, converting);
        }

        [TestMethod]
        public void ConversionEndedEventTest()
        {
            MediaFileDownloader downloader = new MediaFileDownloader();

            MediaFile file = MediaFile.CreateNew(VboxDownloadVideo);
            string downloadedPath = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).DownloadPath;
            bool converting = false;
            downloader.MediaFileConvertionCompelete += delegate { converting = true; };
            downloader.ConvertSingleFile(file, downloadedPath, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), SupportedConversionFormats.Mp3);

            Assert.AreEqual(true, converting);
        }
    }
}
