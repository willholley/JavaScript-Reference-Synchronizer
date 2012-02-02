using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CassetteHelper
{
    public class ApplicationRelativeMatchingStrategy : IMatchingStrategy
    {
        private readonly Regex matchingRegex;

        public ApplicationRelativeMatchingStrategy(string applicationRootDirectory, string originalFilePath)
        {
            if (String.IsNullOrEmpty(applicationRootDirectory)) throw new ArgumentNullException("applicationRootDirectory");
            if (String.IsNullOrEmpty(originalFilePath)) throw new ArgumentNullException("originalFilePath");

            if (!Directory.Exists(applicationRootDirectory)) throw new ArgumentException("Could not find directory '" + applicationRootDirectory + "'", "applicationRootDirectory");

            var applicationRelativePath = originalFilePath.URIRelativeTo(applicationRootDirectory);
            this.matchingRegex = new Regex("^\\W*///\\W*<reference\\W+path=\"~/" + applicationRelativePath + "\"\\W*/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //this.matchingRegex = new Regex("^///\\W+<reference path=\"~/(?<path>[^\"]+)\"\\W+/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
        
        public bool Match(string line)
        {
            return !String.IsNullOrWhiteSpace(line) && matchingRegex.IsMatch(line);
        }
    }
}