using System;
using System.IO;
using System.Threading;
using CassetteHelper;
using MbUnit.Framework;

namespace CassetteHelperTests
{
    [TestFixture]
    public class StreamReplacerTests
    {
        public void AssertFileLastWriteTimeDifference(ILineVisitor visitor, Action<DateTime, DateTime> assert)
        {
            var replacer = new StreamReplacer(visitor, "replaced.js");

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
            public bool Visit(TextReader input, Action<string> onMatch, Action<string> onNonMatch)
            {
                onNonMatch("");
                return false;
            }
        }

        class AlwaysMatchingLineVisitor : ILineVisitor
        {
            public bool Visit(TextReader input, Action<string> onMatch, Action<string> onNonMatch)
            {
                onMatch("");
                return true;
            }
        }
    }
}
