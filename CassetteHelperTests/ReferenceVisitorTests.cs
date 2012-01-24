using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
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
            new ReferenceVisitor(urlToSearchFor);
        }

        [Test, ExpectedArgumentNullException("input")]
        public void WhenNullStreamPassedThrowsArgumentNullException()
        {
            var referenceVisitor = new ReferenceVisitor("test");
            referenceVisitor.FindReferences(null, r => { });
        }

        public bool VisitReferences(string resourceName, string urlToSearchFor)
        {
            var referenceVisitor = new ReferenceVisitor(urlToSearchFor);
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("CassetteHelperTests.Resources." + resourceName))
            {
                using (var reader = new StreamReader(input))
                {
                    bool visited = false;
                    referenceVisitor.FindReferences(reader, r => visited = true);

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
        
        public class ReferenceVisitor
        {
            readonly Regex reference = new Regex("^///\\W+<reference path=\"(?<path>[^\"]+)\"\\W+/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            private readonly string urlToSearchFor;

            public ReferenceVisitor(string urlToSearchFor)
            {
                if (string.IsNullOrEmpty(urlToSearchFor)) throw new ArgumentNullException("urlToSearchFor");

                this.urlToSearchFor = urlToSearchFor.ToLowerInvariant();
            }

            public void FindReferences(StreamReader input, Action<string> onVisit)
            {
                if(input == null) throw new ArgumentNullException("input");

                string line;
                while ((line = input.ReadLine()) != null)
                {
                    var match = reference.Match(line);
                    if (match.Success && match.Groups["path"].Value.ToLowerInvariant() == urlToSearchFor)
                    {
                        onVisit(line);
                    }
                }
            }
        }
    }
}
