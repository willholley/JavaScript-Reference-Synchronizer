using System;
using System.Text.RegularExpressions;

namespace JSReferenceSynchronizer.Replacement
{
    public class ApplicationRelativeReplacementStrategy : IReplacementStrategy
    {
        private readonly string newReference;
        private Regex matcher;

        public ApplicationRelativeReplacementStrategy(string applicationRootDirectory, string originalFilePath, string newFilePath)
        {
            this.matcher = ReferenceRegex.ApplicationRelative(applicationRootDirectory, originalFilePath);

            var replacementApplicationRelativePath = newFilePath.URIRelativeTo(applicationRootDirectory);
            this.newReference = string.Format("/// <reference path=\"~/{0}\" />", replacementApplicationRelativePath); 
        }

        public void SetContext(string targetFileFullName)
        {
            // no op
        }

        public string Replace(string line)
        {
            if(String.IsNullOrWhiteSpace(line))
            {
                return line;
            }
            
            return matcher.IsMatch(line) ? newReference : line;
        }
    }
}