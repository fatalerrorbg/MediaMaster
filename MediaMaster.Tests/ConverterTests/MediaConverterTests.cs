using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaMaster.Converter;
using MediaMaster.Tests.DownloaderTests.VboxTests;
using MediaMaster.Ffmpeg;

namespace MediaMaster.Tests.ConverterTests
{
    [TestClass]
    public class MediaConverterTests
    {
        [TestMethod]
        public void IsInitializedTest()
        {
            FfmpegManager manager = new FfmpegManager();

            Assert.AreEqual(Path.Combine(Path.GetTempPath(), "ffmpeg"), manager.FfmpegDelployPath);
        }

        [TestMethod]
        public void IsEnsuringFfmpegCorrectlyTest()
        {
            FfmpegManager manager = new FfmpegManager();

            manager.EnsureFfmpeg();

            Assert.IsTrue(File.Exists(Path.Combine(manager.FfmpegDelployPath, manager.FfmpegFileName)));
        }

        [TestMethod]
        public void IsConvertingToMp3()
        {
            MediaDownloader downloader = new MediaDownloader();
            MediaFile mediaFile = MediaFile.CreateNew(VboxDownloadConvertTests.VboxDownloadVideo);
            string existingPath = Path.Combine(Directory.GetCurrentDirectory(), mediaFile.GetMetadata().FileName + mediaFile.GetMetadata().FileExtension);
            if (!File.Exists(existingPath))
            {
                downloader.Download(mediaFile, Directory.GetCurrentDirectory());
            }

            MediaConverter converter = new MediaConverter();
            ConvertResult result = converter.Convert(mediaFile, existingPath, Directory.GetCurrentDirectory(), new MediaConverterMetadata(Bitrates.Kbps192));

            Assert.IsTrue(result.IsConverted);
            Assert.IsTrue(File.Exists(result.ConvertedPath));
        }

    }
}
