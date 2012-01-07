using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace CassetteHelperTests
{
    [TestFixture]
    public class ReferenceFinderTests
    {
        private ReferenceFinder referenceFinder = new ReferenceFinder();

        [Test, ExpectedArgumentNullException("input")]
        public void WhenNullStreamPassedThrowsArgumentNullException()
        {
            referenceFinder.FindReferences(null);
        }

        [Test]
        public void WhenNoReferencesReturnsFalse()
        {
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("CassetteHelperTests.Resources.NoReferences.js"))
            {
                using (var reader = new StreamReader(input))
                {
                    referenceFinder.FindReferences(reader);
                }
            }
        }

        public class ReferenceFinder
        {
            public bool FindReferences(StreamReader input)
            {
                if(input == null) throw new ArgumentNullException("input");

                return false;
            }
        }
    }
}
