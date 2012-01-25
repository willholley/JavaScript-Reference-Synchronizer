using System;
using System.IO;
using System.Threading;
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
            var replacer = new LineReplacer(new NeverMatchingLineVisitor(), "replaced.js");

            using(var temporaryFile = CreateTemporaryFileFrom.EmbeddedResource("NoReferences.js"))
            {
                var originalWriteTime = File.GetLastWriteTimeUtc(temporaryFile.AbsolutePath);
                Thread.Sleep(100); // delay to ensure new write time is correct

                replacer.Replace(temporaryFile.AbsolutePath);

                Assert.AreEqual(originalWriteTime, File.GetLastWriteTimeUtc(temporaryFile.AbsolutePath));
            }
        }

        class NeverMatchingLineVisitor : ILineVisitor
        {
            public bool Visit(StreamReader input, Action<string> onMatch, Action<string> onNonMatch)
            {
                onNonMatch("");
                return false;
            }
        }

        class AlwaysMatchingLineVisitor : ILineVisitor
        {
            public bool Visit(StreamReader input, Action<string> onMatch, Action<string> onNonMatch)
            {
                onMatch("");
                return true;
            }
        }

        [Test]
        public void ModifiesFileWithMatch()
        {
            var replacer = new LineReplacer(new AlwaysMatchingLineVisitor(), "replaced");

            using (var temporaryFile = CreateTemporaryFileFrom.EmbeddedResource("SingleReference.js"))
            {
                var originalWriteTime = File.GetLastWriteTimeUtc(temporaryFile.AbsolutePath);
                Thread.Sleep(100); // delay to ensure new write time is correct

                replacer.Replace(temporaryFile.AbsolutePath);
                
                Assert.LessThan(originalWriteTime, File.GetLastWriteTimeUtc(temporaryFile.AbsolutePath));
            }
        }
    }

    public class LineReplacer
    {
        private readonly string newReference;
        private readonly ILineVisitor visitor;

        public LineReplacer(ILineVisitor visitor, string replacementUrl)
        {
            this.visitor = visitor;
            this.newReference = string.Format("/// <reference path=\"{0}\" />", replacementUrl); 
        }
        
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
