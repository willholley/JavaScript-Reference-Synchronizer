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
        private ReferenceVisitor referenceVisitor = new ReferenceVisitor();

        [Test, ExpectedArgumentNullException("input")]
        public void WhenNullStreamPassedThrowsArgumentNullException()
        {
            referenceVisitor.FindReferences(null, r => { });
        }

        public bool VisitedReferences(string resourceName)
        {
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
            Assert.IsFalse(VisitedReferences("NoReferences.js"));
        }

        [Test]
        public void OnVisitCalledWhenApplicationRelativeReferenceFound()
        {
            Assert.IsFalse(VisitedReferences("SingleApplicationRelativeReference.js"));
        }

        public class ReferenceVisitor
        {
            readonly Regex applicationRelativeReference = new Regex("^///\\W+<reference path=\"~/(?<path>[^\"]+)\"\\W+/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	   
            public void FindReferences(StreamReader input, Action<string> onVisit)
            {
                if(input == null) throw new ArgumentNullException("input");

                string line;
                while ((line = input.ReadLine()) != null)
                {
                    var match = applicationRelativeReference.Match(line);
                    if (match.Success)// && match.Groups["path"].Value.ToLowerInvariant() == oldUrl)
                    {
                        onVisit(line);
                    }
                }
            }
        }
    }
}
