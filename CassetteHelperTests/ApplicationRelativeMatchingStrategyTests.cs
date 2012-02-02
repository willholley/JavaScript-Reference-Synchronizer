using System;
using System.IO;
using System.Text.RegularExpressions;
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

    public class ApplicationRelativeMatchingStrategy : IMatchingStrategy
    {
        private readonly Regex matchingRegex;

        public ApplicationRelativeMatchingStrategy(string applicationRootDirectory, string originalFilePath)
        {
            if (String.IsNullOrEmpty(applicationRootDirectory)) throw new ArgumentNullException("applicationRootDirectory");
            if (String.IsNullOrEmpty(originalFilePath)) throw new ArgumentNullException("originalFilePath");

            if (!Directory.Exists(applicationRootDirectory)) throw new ArgumentException("Could not find directory '" + applicationRootDirectory + "'", "applicationRootDirectory");

            var applicationRelativePath = originalFilePath.URIRelativeTo(applicationRootDirectory);
            this.matchingRegex = new Regex("^\\W*///\\W*<reference\\W+path=\"~/" + applicationRelativePath + "\"\\W*/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //this.matchingRegex = new Regex("^///\\W+<reference path=\"~/(?<path>[^\"]+)\"\\W+/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
        
        public bool Match(string line)
        {
            return !String.IsNullOrWhiteSpace(line) && matchingRegex.IsMatch(line);
        }
    }
}