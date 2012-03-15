using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CassetteHelper.Matching
{
    public class ApplicationRelativeMatcher
    {
        protected readonly Regex matchingRegex;

        public ApplicationRelativeMatcher(string applicationRootDirectory, string originalFilePath)
        {
            if (String.IsNullOrEmpty(applicationRootDirectory)) throw new ArgumentNullException("applicationRootDirectory");
            if (String.IsNullOrEmpty(originalFilePath)) throw new ArgumentNullException("originalFilePath");

            if (!Directory.Exists(applicationRootDirectory)) throw new ArgumentException("Could not find directory '" + applicationRootDirectory + "'", "applicationRootDirectory");

            var applicationRelativePath = originalFilePath.URIRelativeTo(applicationRootDirectory);
            this.matchingRegex = new Regex("^\\W*///\\W*<reference\\W+path=\"~/" + applicationRelativePath + "\"\\W*/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(string line)
        {
            return matchingRegex.IsMatch(line);
        }
    }
}