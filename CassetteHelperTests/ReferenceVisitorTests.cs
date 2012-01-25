using System.IO;
using System.Reflection;
using CassetteHelper;
using MbUnit.Framework;

namespace CassetteHelperTests
{
    [TestFixture]
    public class ReferenceVisitorTests
    {
        [Test, ExpectedArgumentNullException("urlToSearchFor")]
        [Row("")]
        [Row(null)]
        public void WhenNullOrEmptyUrlToSearchForThrowsArgumentNullException(string urlToSearchFor)
        {
            new MatchingLineVisitor(urlToSearchFor);
        }

        [Test, ExpectedArgumentNullException("input")]
        public void WhenNullStreamPassedThrowsArgumentNullException()
        {
            var referenceVisitor = new MatchingLineVisitor("test");
            referenceVisitor.Visit(null, r => { }, r => { });
        }

        public bool VisitReferences(string resourceName, string urlToSearchFor)
        {
            var referenceVisitor = new MatchingLineVisitor(urlToSearchFor);
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("CassetteHelperTests.Resources." + resourceName))
            {
                using (var reader = new StreamReader(input))
                {
                    bool visited = false;
                    referenceVisitor.Visit(reader, r => visited = true, r => { });

                    return visited;
                }
            }
        }

        [Test]
        public void WhenNoReferencesDoesNotCallVisitor()
        {
            Assert.IsFalse(VisitReferences("NoReferences.js", "test"));
        }

        [Test]
        public void OnVisitCalledWhenApplicationRelativeReferenceFound()
        {
            Assert.IsTrue(VisitReferences("SingleReference.js", "~/Scripts/First.js"));
        }

        [Test]
        public void OnVisitNotCalledWhenNonMatchingApplicationRelativeReferenceFound()
        {
            Assert.IsFalse(VisitReferences("SingleReference.js", "test"));
        }

        [Test]
        public void OnVisitNotCalledWhenReferenceIsNotAValidComment()
        {
            Assert.IsFalse(VisitReferences("NotAValidReference.js", "~/Scripts/First.js"));
        }
    }
}
