using System;
using System.Collections.Generic;
using MediaMaster.Resolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MediaMaster.Tests.Resolver
{
    [TestClass]
    public class VboxResolverTests
    {
        [TestMethod]
        public void IsResolvingBasicTest()
        {
            VboxResolver resolver = new VboxResolver();
            IEnumerable<string> urls = resolver.ResolveByName("bon jovi its my life HD");

            MediaFile file = MediaFile.CreateNew(urls.First());

            Assert.IsTrue(file.Metadata.FileName.ToLower().Contains("bon jovi"));
        }
    }
}
