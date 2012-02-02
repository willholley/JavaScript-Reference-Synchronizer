using System;
using CassetteHelper;
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
            new ApplicationRelativeMatchingStrategy(applicationRootDirectory, "original.js");
        }

        [Test, ExpectedArgumentNullException("originalFilePath")]
        [Row(null)]
        [Row("")]
        public void OriginalFilePathMustNotBeNullOrEmpty(string originalFilePath)
        {
            new ApplicationRelativeMatchingStrategy(@"c:\", originalFilePath);
        }

        [Test]
        [Row("/// not a match", false)]
        [Row(null, false)]
        [Row("", false)]
        [Row("function foo()", false)]
        [Row("/// <reference path=\"~/test/original.js\" />", true)]
        [Row("/// <reference path=\"~/Test/Original.js\" />", true)]        // case insensitive
        [Row("  /// <reference path=\"~/test/original.js\" />", true)]    // whitespace at beginning
        [Row("///<reference  path=\"~/test/original.js\" />", true)]        // whitespace before path
        [Row("///<reference path=\"~/test/original.js\"  />", true)]        // whitespace before close tag
        [Row("///<reference path=\"~/test/original.js\"/>", true)]          // no whitespace before close tag
        [Row("function foo() /// <reference path=\"~/test/original.js\" />", false)]
        [Row("/// <reference path=\"~/test/notoriginal.js\" />", false)]    // wrong file
        public void Match(string line, bool expected)
        {
            var strategy = new ApplicationRelativeMatchingStrategy(@"c:\", @"c:\test\original.js");
            Assert.AreEqual(expected, strategy.Match(line));
        }
    }
}