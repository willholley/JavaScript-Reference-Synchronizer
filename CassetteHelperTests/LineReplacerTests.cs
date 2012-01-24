using System;
using System.IO;
using System.Text.RegularExpressions;
using CassetteHelper;
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

        [Test]
        public void ModifiesFileWithMatch()
        {
            const string original = "~/Scripts/First.js";
            const string replaced = "~/Scripts/Replaced.js";

            var replacer = new LineReplacer(original, replaced);

            using (var temporaryFile = CreateTemporaryFileFrom.EmbeddedResource("SingleReference.js"))
            {
                AssertHasReference(original, temporaryFile);

                replacer.Replace(temporaryFile.AbsolutePath);

                AssertHasReference(replaced, temporaryFile);
            }
        }

        private void AssertHasReference(string original, TemporaryFile temporaryFile)
        {
            using (var file = File.OpenText(temporaryFile.AbsolutePath))
            {
                Assert.IsTrue(new ReferenceVisitor(original).HasReference(file));
            }
        }
    }

    public class LineReplacer
    {
        public LineReplacer(string originalUrl, string replacementUrl)
        {
            this.visitor = new ReferenceVisitor(originalUrl);
            this.newReference = string.Format("/// <reference path=\"{0}\" />", replacementUrl); 
        }

        private readonly string newReference;
        private readonly ReferenceVisitor visitor;

        public void Replace(string targetFilePath)
        {
            if (string.IsNullOrEmpty(targetFilePath)) throw new ArgumentNullException("targetFilePath");
            if (!File.Exists(targetFilePath)) throw new FileNotFoundException("Could not find file to replace lines in", targetFilePath);
            
            using (var stringWriter = new StringWriter())
            {
                bool containsReferences;

                using (var inputStream = File.OpenText(targetFilePath))
                {
                    containsReferences = visitor.Visit(inputStream, 
                        r => stringWriter.WriteLine(this.newReference),
                        stringWriter.WriteLine);
                }

                if (containsReferences)
                {
                    File.WriteAllText(targetFilePath, stringWriter.ToString());
                }
            }
        }
    }
}
