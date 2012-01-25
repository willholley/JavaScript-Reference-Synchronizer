using System;
using System.IO;
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
            new ApplicationRelativeMatchingStrategy(applicationRootDirectory, "original.js", "new.js");
        }

        [Test, ExpectedArgumentNullException("originalFilePath")]
        [Row(null)]
        [Row("")]
        public void OriginalFilePathMustNotBeNullOrEmpty(string originalFilePath)
        {
            new ApplicationRelativeMatchingStrategy(@"c:\", originalFilePath, "new.js");
        }

        [Test, ExpectedArgumentNullException("newFilePath")]
        [Row(null)]
        [Row("")]
        public void NewFilePathMustNotBeNullOrEmpty(string newFilePath)
        {
            new ApplicationRelativeMatchingStrategy(@"c:\", "original.js", newFilePath);
        }
    }

    public class ApplicationRelativeMatchingStrategy
    {
        public ApplicationRelativeMatchingStrategy(string applicationRootDirectory, string originalFilePath, string newFilePath)
        {
            if (String.IsNullOrEmpty(applicationRootDirectory)) throw new ArgumentNullException("applicationRootDirectory");
            if (String.IsNullOrEmpty(originalFilePath)) throw new ArgumentNullException("originalFilePath");
            if (String.IsNullOrEmpty(newFilePath)) throw new ArgumentNullException("newFilePath");

            if (!Directory.Exists(applicationRootDirectory)) throw new ArgumentException("Could not find directory '" + applicationRootDirectory + "'", "applicationRootDirectory");

        }


    }
}