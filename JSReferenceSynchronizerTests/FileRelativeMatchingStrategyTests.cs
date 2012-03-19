using MbUnit.Framework;

namespace JSReferenceSynchronizer.Tests
{
    [TestFixture]
    public class FileRelativeMatchingStrategyTests
    {
        [Test]
        [Row(@"c:\test.js", "/// not a match", "/// not a match")]
        [Row(@"c:\test.js", null, null)]
        [Row(@"c:\test.js", "", "")]
        [Row(@"c:\test.js", "function foo()", "function foo()")]
        [Row(@"c:\test.js", "/// <reference path=\"~/test/original.js\" />", "/// <reference path=\"~/test/original.js\" />")] // application relative should not match
        [Row(@"c:\test.js", "/// <reference path=\"test/original.js\" />", "/// <reference path=\"test/replacement.js\" />")]
        [Row(@"c:\test.js", "/// <reference path=\"test\\original.js\" />", "/// <reference path=\"test/replacement.js\" />")]
        [Row(@"c:\test\test.js", "/// <reference path=\"original.js\" />", "/// <reference path=\"replacement.js\" />")]
        [Row(@"c:\test.js", "function foo() /// <reference path=\"~/test/original.js\" />", "function foo() /// <reference path=\"~/test/original.js\" />")]
        [Row(@"c:\test.js", "/// <reference path=\"test/notoriginal.js\" />", "/// <reference path=\"test/notoriginal.js\" />")]    // wrong file
        public void Replace(string context, string line, string expected)
        {
            var strategy = new FileRelativeMatchingStrategy(@"c:\test\original.js", @"c:\test\replacement.js");
            strategy.SetContext(context);

            Assert.AreEqual(expected, strategy.Replace(line));
        }
    }
}