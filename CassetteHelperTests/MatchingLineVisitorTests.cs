using System;
using System.IO;
using System.Reflection;
using CassetteHelper;
using MbUnit.Framework;

namespace CassetteHelperTests
{
    //[TestFixture]
    //public class MatchingLineVisitorTests
    //{
    //    [Test, ExpectedArgumentNullException("urlToSearchFor")]
    //    [Row("")]
    //    [Row(null)]
    //    public void WhenNullOrEmptyUrlToSearchForThrowsArgumentNullException(string urlToSearchFor)
    //    {
    //        new MatchingLineVisitor(urlToSearchFor);
    //    }

    //    [Test, ExpectedArgumentNullException("input")]
    //    public void WhenNullStreamPassedThrowsArgumentNullException()
    //    {
    //        var lineVisitor = new MatchingLineVisitor("test");
    //        lineVisitor.Visit(null, r => { }, r => { });
    //    }

    //    public bool AssertVisited(string resourceName, string urlToSearchFor)
    //    {
    //        var lineVisitor = new MatchingLineVisitor(urlToSearchFor);
    //        using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("CassetteHelperTests.Resources." + resourceName))
    //        {
    //            using (var reader = new StreamReader(input))
    //            {
    //                bool visited = false;
    //                lineVisitor.Visit(reader, r => visited = true, r => { });

    //                return visited;
    //            }
    //        }
    //    }

    //    [Test]
    //    public void WhenNoMatchDoesNotCallVisitor()
    //    {

    //        Assert.IsFalse(AssertVisited("NoReferences.js", "test"));
    //    }

    //    [Test]
    //    public void OnVisitCalledWhenApplicationRelativeReferenceFound()
    //    {
    //        Assert.IsTrue(AssertVisited("SingleReference.js", "~/Scripts/First.js"));
    //    }

    //    [Test]
    //    public void OnVisitNotCalledWhenNonMatchingApplicationRelativeReferenceFound()
    //    {
    //        Assert.IsFalse(AssertVisited("SingleReference.js", "test"));
    //    }

    //    [Test]
    //    public void OnVisitNotCalledWhenReferenceIsNotAValidComment()
    //    {
    //        Assert.IsFalse(AssertVisited("NotAValidReference.js", "~/Scripts/First.js"));
    //    }
    //}


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
