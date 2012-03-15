using System;
using CassetteHelper;
using CassetteHelper.Matching;
using MbUnit.Framework;

namespace CassetteHelperTests
{
    [TestFixture]
    public class ApplicationRelativeMatchingStrategyTests
    {
        [Test]
        [Row(null, ExpectedException = typeof(ArgumentNullException), ExpectedExceptionMessage = "applicationRootDirectory")]
        [Row("", ExpectedException = typeof(ArgumentNullException), ExpectedExceptionMessage = "applicationRootDirectory")]
        [Row(@"c:\iAmAFile.js", ExpectedException = typeof(ArgumentException), ExpectedExceptionMessage = "applicationRootDirectory")]
        [Row(@"c:\doesNotExist", ExpectedException = typeof(ArgumentException), ExpectedExceptionMessage = "applicationRootDirectory")]
        public void ApplicationRootMustBeValid(string applicationRootDirectory)
        {
            new ApplicationRelativeReplacementStrategy(applicationRootDirectory, "original.js", "replacement.js");
        }

        [Test, ExpectedArgumentNullException("originalFilePath")]
        [Row(null)]
        [Row("")]
        public void OriginalFilePathMustNotBeNullOrEmpty(string originalFilePath)
        {
            new ApplicationRelativeReplacementStrategy(@"c:\", originalFilePath, "replacement.js");
        }

        [Test]
        [Row("/// not a match", "/// not a match")]
        [Row(null, null)]
        [Row("", "")]
        [Row("function foo()", "function foo()")]
        [Row("/// <reference path=\"~/test/original.js\" />", "/// <reference path=\"~/test/replacement.js\" />")]
        [Row("/// <reference path=\"~/Test/Original.js\" />", "/// <reference path=\"~/test/replacement.js\" />")]        // case insensitive
        [Row("  /// <reference path=\"~/test/original.js\" />", "/// <reference path=\"~/test/replacement.js\" />")]    // whitespace at beginning
        [Row("///<reference  path=\"~/test/original.js\" />", "/// <reference path=\"~/test/replacement.js\" />")]        // whitespace before path
        [Row("///<reference path=\"~/test/original.js\"  />", "/// <reference path=\"~/test/replacement.js\" />")]        // whitespace before close tag
        [Row("///<reference path=\"~/test/original.js\"/>", "/// <reference path=\"~/test/replacement.js\" />")]          // no whitespace before close tag
        [Row("function foo() /// <reference path=\"~/test/original.js\" />", "function foo() /// <reference path=\"~/test/original.js\" />")]
        [Row("/// <reference path=\"~/test/notoriginal.js\" />", "/// <reference path=\"~/test/notoriginal.js\" />")]    // wrong file
        public void Replace(string line, string expected)
        {
            var strategy = new ApplicationRelativeReplacementStrategy(@"c:\", @"c:\test\original.js", @"c:\test\replacement.js");

            Assert.AreEqual(expected, strategy.Replace(line));
        }
    }
}