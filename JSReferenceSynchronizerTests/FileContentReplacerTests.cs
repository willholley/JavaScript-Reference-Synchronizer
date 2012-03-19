using System;
using System.IO;
using System.Threading;
using JSReferenceSynchronizer.Replacement;
using MbUnit.Framework;

namespace JSReferenceSynchronizer.Tests
{
    [TestFixture]
    public class FileContentReplacerTests
    {
        public void AssertFileLastWriteTimeDifference(IReplacementStrategy replacementStrategy, Action<DateTime, DateTime> assert)
        {
            var replacer = new FileContentReplacer(replacementStrategy);

            var temporaryFilePath = Path.GetTempFileName();
            File.WriteAllText(temporaryFilePath, "dummy line");
            using (var temporaryFile = new TemporaryFile(temporaryFilePath))
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
            AssertFileLastWriteTimeDifference(new NeverMatchStrategy(), Assert.AreEqual);
        }

        [Test]
        public void ModifiesFileWithMatch()
        {
            AssertFileLastWriteTimeDifference(new AlwaysMatchStrategy(), Assert.LessThan);
        }

        class NeverMatchStrategy : IReplacementStrategy
        {
            public void SetContext(string targetFileFullName)
            {
                // no op
            }

            public string Replace(string line)
            {
                return line;
            }
        }

        class AlwaysMatchStrategy : IReplacementStrategy
        {
            public void SetContext(string targetFileFullName)
            {
                // no op
            }

            public string Replace(string line)
            {
                return "replaced";
            }
        }
    }
}
