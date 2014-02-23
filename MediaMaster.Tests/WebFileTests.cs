using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaMaster.Tests
{
    [TestClass]
    public class WebFileTests
    {
        public const string VboxTestUrl = "http://www.vbox7.com/play:a40c203d8b";

        [TestMethod]
        public void PropertyUrlIsSetCorrectlyTest()
        {
            WebFile webFile = new VboxFile(VboxTestUrl);

            Assert.AreEqual(webFile.Url, VboxTestUrl);
        }

        [TestMethod]
        public void PropertyFileOriginIsSetCorrectlyTest()
        {
            WebFile webFile = new VboxFile(VboxTestUrl);

            Assert.AreEqual(webFile.FileOrigin, FileOrigin.Vbox7);
        }

        [TestMethod]
        public void GetsCorrectDownloaderTestVbox()
        {
            WebFile webFile = WebFile.CreateNew(VboxTestUrl);

            bool isVboxDownloader = webFile is VboxFile;

            Assert.AreEqual(true, isVboxDownloader);
        }

        [TestMethod]
        public void GetsCorrectMetadataTest()
        {
            WebFile webFile = WebFile.CreateNew(VboxTestUrl);
            WebFileMetadata metadata = webFile.GetMetadata();

            bool isVboxMetadata = metadata is VboxFileMetadata;

            Assert.AreEqual(true, isVboxMetadata);
        }
    }
}
