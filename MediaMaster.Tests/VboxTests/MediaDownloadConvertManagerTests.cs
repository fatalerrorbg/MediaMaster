using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaMaster.Tests.DownloaderTests.VboxTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;

namespace MediaMaster.Tests.MediaDownloadConvertManagerTests
{
    [TestClass]
    public class MediaDownloadConvertManagerTests
    {
        [TestMethod]
        public void IsDownloadingBasicTest()
        {
            MediaDownloadConvertManager manager = new MediaDownloadConvertManager();

            for (int i = 0; i < 8; i++)
            {
                VboxFile vboxFile = (VboxFile)MediaFile.CreateNew(VboxDownloadConvertTests.VboxDownloadVideo);
                vboxFile.Metadata.FileName = "file " + i;
                manager.EnqueueDownloadAndConvertRequest(vboxFile,
                    Directory.GetCurrentDirectory(),
                    new MediaConverterMetadata(Converter.Bitrates.Kbps192, "file " + i, SupportedConversionFormat.Mp3));
            }

            bool done = false;

            int count = 0;
            manager.DownloadConvertResult += delegate { done = ++count == 8; };
            manager.StartDownload();

            while (!done)
            {
                Thread.Sleep(200);
            }

            Assert.IsTrue(done);
        }
    }
}
