using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaMaster.Tests
{
    [TestClass]
    public class VboxFileTests
    {
        [TestMethod]
        public void GetsCorrectVideoIdTest()
        {
            VboxFile file = WebFile.CreateNew(WebFileTests.VboxTestUrl) as VboxFile;
            string id = "a40c203d8b";

            Assert.AreEqual(file.Metadata.VideoId, id);
        }

        [TestMethod]
        public void GetsCorrectVideoDownloadLinkTest()
        {
            WebFile downloader = WebFile.CreateNew(WebFileTests.VboxTestUrl);

            string downloadLink = downloader.GetMetadata().DownloadLink;

            Assert.AreEqual("http://media09.vbox7.com/s/a4/a40c203d8br20619d829.mp4", downloadLink);
        }

        [TestMethod]
        public void GetsCorrectThumbnailLinkTest()
        {
            WebFile downloader = WebFile.CreateNew(WebFileTests.VboxTestUrl);

            string thumbnailLink = downloader.GetMetadata().ThumbnailLink;

            Assert.AreEqual("i49.vbox7.com/o/a40/a40c203d8b0.jpg", thumbnailLink);
        }

        [TestMethod]
        public void GetsCorrectFileNameTest()
        {
            string fileName = "Как се наказва изневяра .. Смях";

            VboxFile file = WebFile.CreateNew(WebFileTests.VboxTestUrl) as VboxFile;

            string actualName = file.Metadata.FileName;

            Assert.AreEqual(fileName, actualName);
        }
    }
}
