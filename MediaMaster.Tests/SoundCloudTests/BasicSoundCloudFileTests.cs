using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaMaster.Tests.SoundCloudTests
{
    [TestClass]
    public class BasicSoundCloudFileTests
    {
        public const string SoundCloudLink = "https://soundcloud.com/r-kellyofficial/r-kelly-genius";

        [TestMethod]
        public void IsFileCretedCorrectTest()
        {
            MediaFile file = MediaFile.CreateNew(SoundCloudLink);

            Assert.IsTrue(file is SoundCloudFile);
        }

        [TestMethod]
        public void IsOriginSoundCloud()
        {
            MediaFile file = MediaFile.CreateNew(SoundCloudLink);

            Assert.IsTrue(file.FileOrigin == FileOrigin.SoundCloud);
        }

        [TestMethod]
        public void IsInitializingMetadataCorrectly()
        {
            MediaFile file = MediaFile.CreateNew(SoundCloudLink);

            file.InitializeMetadata();
        }
    }
}
