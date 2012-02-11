using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CassetteHelper
{
    public class ApplicationRelativeReplacementStrategy : IReplacementStrategy
    {
        private readonly Regex matchingRegex;
        private string newReference;

        public ApplicationRelativeReplacementStrategy(string applicationRootDirectory, string originalFilePath, string newFilePath)
        {
            if (String.IsNullOrEmpty(applicationRootDirectory)) throw new ArgumentNullException("applicationRootDirectory");
            if (String.IsNullOrEmpty(originalFilePath)) throw new ArgumentNullException("originalFilePath");

            if (!Directory.Exists(applicationRootDirectory)) throw new ArgumentException("Could not find directory '" + applicationRootDirectory + "'", "applicationRootDirectory");

            var applicationRelativePath = originalFilePath.URIRelativeTo(applicationRootDirectory);
            this.matchingRegex = new Regex("^\\W*///\\W*<reference\\W+path=\"~/" + applicationRelativePath + "\"\\W*/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //this.matchingRegex = new Regex("^///\\W+<reference path=\"~/(?<path>[^\"]+)\"\\W+/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var replacementApplicationRelativePath = newFilePath.URIRelativeTo(applicationRootDirectory);
            this.newReference = string.Format("/// <reference path=\"~/{0}\" />", replacementApplicationRelativePath); 
        }
        
        public string Replace(string line)
        {
            if(String.IsNullOrWhiteSpace(line))
            {
                return line;
            }

            return matchingRegex.Replace(line, newReference);
        }
    }
}