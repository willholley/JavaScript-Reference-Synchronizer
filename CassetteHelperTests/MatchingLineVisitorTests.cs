using System;
using System.IO;
using CassetteHelper;
using MbUnit.Framework;

namespace CassetteHelperTests
{
    [TestFixture]
    public class MatchingLineVisitorTests
    {
        class FixedMatchingStrategy : IMatchingStrategy
        {
            private readonly bool result;

            public FixedMatchingStrategy(bool result)
            {
                this.result = result;
            }

            public bool Match(string line)
            {
                return this.result;
            }
        }

        [Test, ExpectedArgumentNullException("input")]
        public void WhenNullStreamPassedThrowsArgumentNullException()
        {
            var lineVisitor = new MatchingLineVisitor(new FixedMatchingStrategy(true));
            lineVisitor.Visit(null, r => { }, r => { });
        }
        
        [Test]
        [Row(true)]
        [Row(false)]
        public void OnVisitCallMatchesStrategy(bool expected)
        {
            var lineVisitor = new MatchingLineVisitor(new FixedMatchingStrategy(expected));
            using (var input = new StringReader("one" + Environment.NewLine + "two"))
            {
                bool onVisitCalled = false;
                lineVisitor.Visit(input, r => onVisitCalled = true, r => { });

                Assert.AreEqual(expected, onVisitCalled);
            }
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void OnNotVisitCallMatchesStrategy(bool expected)
        {
            var lineVisitor = new MatchingLineVisitor(new FixedMatchingStrategy(!expected));
            using (var input = new StringReader("one" + Environment.NewLine + "two"))
            {
                bool onNotVisitCalled = false;
                lineVisitor.Visit(input, r => { }, r => onNotVisitCalled = true);

                Assert.AreEqual(expected, onNotVisitCalled);
            }
        }
    }
}
