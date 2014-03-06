using System;
using MediaMaster.Resolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaMaster.Tests.Resolver
{
    [TestClass]
    public class VboxResolverTests
    {
        [TestMethod]
        public void IsResolvingBasicTest()
        {
            VboxResolver resolver = new VboxResolver();
            string url = resolver.ResolveByName("bon jovi its my life HD");

            MediaFile file = MediaFile.CreateNew(url);

            Assert.IsTrue(file.Metadata.FileName.ToLower().Contains("bon jovi"));
        }
    }
}
