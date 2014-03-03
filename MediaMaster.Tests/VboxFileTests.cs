using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MediaMaster.Tests
{
    [TestClass]
    public class VboxFileTests
    {
        [TestMethod]
        public void GetsCorrectVideoIdTest()
        {
            VboxFile file = MediaFile.CreateNew(MediaFileTests.VboxTestUrl) as VboxFile;
            string id = "a40c203d8b";

            Assert.AreEqual(file.Metadata.VideoId, id);
        }

        [TestMethod]
        public void GetsCorrectVideoDownloadLinkTest()
        {
            MediaFile downloader = MediaFile.CreateNew(MediaFileTests.VboxTestUrl);

            string downloadLink = downloader.GetMetadata().DownloadLink;

            Assert.AreEqual("http://media09.vbox7.com/s/a4/a40c203d8br20619d829.mp4", downloadLink);
        }

        [TestMethod]
        public void GetsCorrectThumbnailLinkTest()
        {
            MediaFile downloader = MediaFile.CreateNew(MediaFileTests.VboxTestUrl);

            string thumbnailLink = downloader.GetMetadata().ThumbnailLink;

            Assert.AreEqual("i49.vbox7.com/o/a40/a40c203d8b0.jpg", thumbnailLink);
        }

        [TestMethod]
        public void GetsCorrectFileNameTest()
        {
            string fileName = "Как се наказва изневяра .. Смях";

            VboxFile file = MediaFile.CreateNew(MediaFileTests.VboxTestUrl) as VboxFile;

            string actualName = file.Metadata.FileName;

            Assert.AreEqual(fileName, actualName);
        }

        [TestMethod]
        public void ResumesDownloadProperly()
        {
            MediaDownloader downloader = new MediaDownloader();

            bool canceled = false;
            bool downloaded = false;
            downloader.MediaFileDownloadProgress += (s, e) =>
            {
                if (((int)e.PercentageComplete >= 25 && (int)e.PercentageComplete <= 30) && !canceled)
                {
                    e.Cancel = canceled = true;
                }

                if (e.PercentageComplete >= 100)
                {
                    downloaded = true;
                }
            };

            MediaFile file = MediaFile.CreateNew(MediaFileTests.VboxTestUrl);
            downloader.Download(file, Directory.GetCurrentDirectory());

            while (!canceled)
            {
                Thread.Sleep(500);
            }

            downloader.Download(file, Directory.GetCurrentDirectory(), true);

            while (!downloaded)
            {
                Thread.Sleep(500);
            }

            Assert.IsTrue(downloaded);
        }
    }
}
