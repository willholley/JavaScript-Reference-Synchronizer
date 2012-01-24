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

        public void FindReferences(StreamReader input, Action<string> onVisit)
        {
            if (input == null) throw new ArgumentNullException("input");

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