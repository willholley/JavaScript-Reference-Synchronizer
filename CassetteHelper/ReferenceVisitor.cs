using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CassetteHelper
{
    public class ReferenceVisitor
    {
        readonly Regex reference = new Regex("^///\\W+<reference path=\"(?<path>[^\"]+)\"\\W+/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly string urlToSearchFor;

        public ReferenceVisitor(string urlToSearchFor)
        {
            if (string.IsNullOrEmpty(urlToSearchFor)) throw new ArgumentNullException("urlToSearchFor");

            this.urlToSearchFor = urlToSearchFor.ToLowerInvariant();
        }

        public bool Visit(StreamReader input, Action<string> onMatch, Action<string> onNonMatch)
        {
            if (input == null) throw new ArgumentNullException("input");

            string line;
            bool containsReferences = false;

            while ((line = input.ReadLine()) != null)
            {
                var match = reference.Match(line);
                if (match.Success && match.Groups["path"].Value.ToLowerInvariant() == urlToSearchFor)
                {
                    containsReferences = true;
                    onMatch(line);
                }
                else
                {
                    onNonMatch(line);
                }
            }

            return containsReferences;
        }

        public bool HasReference(StreamReader input)
        {
            bool match = false;

            Visit(input, r => match = true, r => { });

            return match;
        }
    }
}