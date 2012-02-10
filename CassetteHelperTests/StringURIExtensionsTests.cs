using CassetteHelper;
using MbUnit.Framework;

namespace CassetteHelperTests
{
    [TestFixture]
    public class StringURIExtensionsTests
    {
        [Test]
        [Row(@"c:\", @"c:\foo.js", "foo.js")]
        [Row(@"c:\", @"C:\foo.js", "foo.js")] // case insentitive
        [Row(@"c:\", @"c:\test", "test")] // directory
        [Row(@"c:\", @"c:\test\foo.js", "test/foo.js")] // subdirectory
        [Row(@"c:\test.js", @"c:\foo.js", "foo.js")] // sibling
        [Row(@"c:\test\foo.js", @"c:\foo.js", "../foo.js")] // parent
        [Row(@"c:\test", @"c:\test\inner", "inner")] // directory no trailing slash
        public void URIRelativeTo(string baseFile, string currentFile, string expected)
        {
            Assert.AreEqual(expected, currentFile.URIRelativeTo(baseFile));
        }
    }
}