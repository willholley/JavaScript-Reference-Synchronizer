using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CassetteHelper
{
    public interface ILineVisitor
    {
        bool Visit(StreamReader input, Action<string> onMatch, Action<string> onNonMatch);
    }

    public class MatchingLineVisitor : ILineVisitor
    {
        readonly Regex reference = new Regex("^///\\W+<reference path=\"(?<path>[^\"]+)\"\\W+/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly string urlToSearchFor;

        public MatchingLineVisitor(string urlToSearchFor)
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
    }
}