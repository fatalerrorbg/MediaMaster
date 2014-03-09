using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaMaster.Tests.SoundCloudTests
{
    [TestClass]
    public class SoundCloudDownloadTests
    {
        [TestMethod]
        public void IsDownloadingTest()
        {
            MediaFile file = MediaFile.CreateNew(BasicSoundCloudFileTests.SoundCloudLink);

            MediaDownloader downloader = new MediaDownloader();
            var result = downloader.Download(file, Directory.GetCurrentDirectory());

            Assert.IsTrue(result.IsDownloaded);
            Assert.IsTrue(File.Exists(result.DownloadPath));
        }
    }
}
