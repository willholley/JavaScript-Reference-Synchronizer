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
        public void AssertFileLastWriteTimeDifference(ILineVisitor visitor, Action<DateTime, DateTime> assert)
        {
            var replacer = new LineReplacer(visitor, "replaced.js");

            using(var temporaryFile = new TemporaryFile(Path.GetTempFileName()))
            {
                var originalWriteTime = File.GetLastWriteTimeUtc(temporaryFile.AbsolutePath);
                Thread.Sleep(100); // delay to ensure we pick up any time difference

                replacer.Replace(temporaryFile.AbsolutePath);

                assert(originalWriteTime, File.GetLastWriteTimeUtc(temporaryFile.AbsolutePath));
            }
        }

        [Test]
        public void DoesNotModifyFileWithNoMatches()
        {
            AssertFileLastWriteTimeDifference(new NeverMatchingLineVisitor(), Assert.AreEqual);
        }

        [Test]
        public void ModifiesFileWithMatch()
        {
            AssertFileLastWriteTimeDifference(new AlwaysMatchingLineVisitor(), Assert.LessThan);
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
