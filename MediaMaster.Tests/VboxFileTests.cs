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
            string path = Path.Combine(Directory.GetCurrentDirectory(), file.GetMetadata().FileName + file.GetMetadata().FileExtension);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var firstResult = downloader.Download(file, Directory.GetCurrentDirectory());

            if (!firstResult.IsDownloaded || firstResult.Exceptions.Any())
            {
                throw new Exception();
            }

            var secondDownload = downloader.Download(file, Directory.GetCurrentDirectory(), true);

            if (!secondDownload.IsDownloaded || secondDownload.Exceptions.Any())
            {
                throw new Exception();
            }

            Assert.IsTrue(downloaded);
        }
    }
}
