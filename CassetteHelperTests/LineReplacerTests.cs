using System;
using System.IO;
using MbUnit.Framework;

namespace CassetteHelperTests
{
    [TestFixture]
    public class LineReplacerTests
    {
        [Test]
        public void DoesNotModifyFileWithNoMatches()
        {
            var replacer = new LineReplacer("original.js", "replaced.js");

            using(var temporaryFile = CreateTemporaryFileFrom.EmbeddedResource("NoReferences.js"))
            {
                var originalWriteTime = File.GetLastWriteTimeUtc(temporaryFile.AbsolutePath);

                replacer.Replace(temporaryFile.AbsolutePath);

                Assert.AreEqual(originalWriteTime, File.GetLastWriteTimeUtc(temporaryFile.AbsolutePath));
            }
        }
    }

    public class LineReplacer
    {
        private readonly string originalUrl;
        private readonly string replacementUrl;

        public LineReplacer(string originalUrl, string replacementUrl)
        {
            this.originalUrl = originalUrl;
            this.replacementUrl = replacementUrl;
        }

        public void Replace(string targetFilePath)
        {
            if (string.IsNullOrEmpty(targetFilePath)) throw new ArgumentNullException("targetFilePath");
            if (!File.Exists(targetFilePath)) throw new FileNotFoundException("Could not find file to replace lines in", targetFilePath);


        }
    }
}
