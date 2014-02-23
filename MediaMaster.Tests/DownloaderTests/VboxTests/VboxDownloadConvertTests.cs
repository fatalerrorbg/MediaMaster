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
            WebFileDownloader downloader = new WebFileDownloader();
            WebFile file = WebFile.CreateNew(VboxDownloadVideo);
            string downloadedFile = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).FullName;

            Assert.AreEqual(VboxDownloadedVideoPath, downloadedFile);
        }

        [TestMethod]
        public void DownloadFileStartingEventTest()
        {
            WebFileDownloader downloader = new WebFileDownloader();
            bool fired = false;
            WebFile file = WebFile.CreateNew(VboxDownloadVideo);
            downloader.WebFileDownloadStarting += (s, e) => fired = true;
            string downloadedFile = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).FullName;

            Assert.AreEqual(fired, true);
        }

        [TestMethod]
        public void DownloadFileProgressEventTest()
        {
            WebFileDownloader downloader = new WebFileDownloader();

            bool hasDownloadSize = false;
            bool hasMaxSize = false;
            bool hasProgress = false;

            WebFile file = WebFile.CreateNew(VboxDownloadVideo);
            downloader.WebFileDownloadProgress += (s, e) =>
                {
                    hasDownloadSize = e.DownloadedSize > 0;
                    hasMaxSize = e.FileSize > 0;
                    hasProgress = e.PercentageComplete > 0;
                };

            string downloadedFile = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).FullName;

            Assert.AreEqual(true, hasDownloadSize && hasMaxSize && hasProgress);
        }

        [TestMethod]
        public void DownloadFileCompletedEventTest()
        {
            WebFileDownloader downloader = new WebFileDownloader();

            bool downloaded = false;

            WebFile file = WebFile.CreateNew(VboxDownloadVideo);
            downloader.WebFileDownloadFinished += (s, e) =>
            {
                downloaded = true;
            };

            string downloadedFile = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).FullName;

            Assert.AreEqual(true, downloaded);
        }

        [TestMethod]
        public void ConversionStartingEventTest()
        {
            WebFileDownloader downloader = new WebFileDownloader();

            WebFile file = WebFile.CreateNew(VboxDownloadVideo);
            string downloadedPath = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).FullName;
            bool converting = false;
            downloader.WebFileConversionStarting += delegate { converting = true; };
            downloader.ConvertSingleFile(file, downloadedPath, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), SupportedConversionFormats.Mp3);

            Assert.AreEqual(true, converting);
        }

        [TestMethod]
        public void ConversionEndedEventTest()
        {
            WebFileDownloader downloader = new WebFileDownloader();

            WebFile file = WebFile.CreateNew(VboxDownloadVideo);
            string downloadedPath = downloader.Download(file, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).FullName;
            bool converting = false;
            downloader.WebFileConvertionCompelete += delegate { converting = true; };
            downloader.ConvertSingleFile(file, downloadedPath, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), SupportedConversionFormats.Mp3);

            Assert.AreEqual(true, converting);
        }
    }
}
