using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaMaster.Converter;
using MediaMaster.Tests.DownloaderTests.VboxTests;

namespace MediaMaster.Tests.ConverterTests
{
    [TestClass]
    public class MediaConverterTests
    {
        [TestMethod]
        public void IsInitializedTest()
        {
            MediaConverter converter = new MediaConverter();

            Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Ffmpeg"), converter.FfmpegDelployPath);
        }

        [TestMethod]
        public void IsEnsuringFfmpegCorrectlyTest()
        {
            MediaConverter converter = new MediaConverter();

            converter.EnsureFfmpeg();

            Assert.IsTrue(File.Exists(Path.Combine(converter.FfmpegDelployPath, converter.FfmpegFileName)));
        }

        [TestMethod]
        public void IsConvertingToMp3()
        {
            WebFileDownloader downloader = new WebFileDownloader();
            WebFile webFile = WebFile.CreateNew(VboxDownloadConvertTests.VboxDownloadVideo);
            string existingPath = Path.Combine(Directory.GetCurrentDirectory(), webFile.GetMetadata().FileName + webFile.GetMetadata().FileExtension);
            if (!File.Exists(existingPath))
            {
                downloader.Download(webFile, Directory.GetCurrentDirectory());
            }

            MediaConverter converter = new MediaConverter();
            FileInfo file = converter.ConvertSingleFile(existingPath, Path.Combine(Directory.GetCurrentDirectory(), "file.mp3"), new MediaConverterMetadata(Bitrates.Kbps192));

            Assert.IsTrue(File.Exists(file.FullName));
        }

    }
}
