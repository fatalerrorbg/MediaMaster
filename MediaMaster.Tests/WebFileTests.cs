using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaMaster.Tests
{
    [TestClass]
    public class MediaFileTests
    {
        public const string VboxTestUrl = "http://www.vbox7.com/play:a40c203d8b";

        [TestMethod]
        public void PropertyUrlIsSetCorrectlyTest()
        {
            MediaFile MediaFile = new VboxFile(VboxTestUrl);

            Assert.AreEqual(MediaFile.Url, VboxTestUrl);
        }

        [TestMethod]
        public void PropertyFileOriginIsSetCorrectlyTest()
        {
            MediaFile MediaFile = new VboxFile(VboxTestUrl);

            Assert.AreEqual(MediaFile.FileOrigin, FileOrigin.Vbox7);
        }

        [TestMethod]
        public void GetsCorrectDownloaderTestVbox()
        {
            MediaFile MediaFile = MediaFile.CreateNew(VboxTestUrl);

            bool isVboxDownloader = MediaFile is VboxFile;

            Assert.AreEqual(true, isVboxDownloader);
        }

        [TestMethod]
        public void GetsCorrectMetadataTest()
        {
            MediaFile MediaFile = MediaFile.CreateNew(VboxTestUrl);
            MediaFileMetadata metadata = MediaFile.Metadata;

            bool isVboxMetadata = metadata is VboxFileMetadata;

            Assert.AreEqual(true, isVboxMetadata);
        }
    }
}
