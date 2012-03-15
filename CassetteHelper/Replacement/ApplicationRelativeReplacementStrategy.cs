using System;

namespace CassetteHelper.Matching
{
    public class ApplicationRelativeReplacementStrategy : IReplacementStrategy
    {
        private readonly string newReference;
        private ApplicationRelativeMatcher matcher;

        public ApplicationRelativeReplacementStrategy(string applicationRootDirectory, string originalFilePath, string newFilePath)
        {
            this.matcher = new ApplicationRelativeMatcher(applicationRootDirectory, originalFilePath);

            var replacementApplicationRelativePath = newFilePath.URIRelativeTo(applicationRootDirectory);
            this.newReference = string.Format("/// <reference path=\"~/{0}\" />", replacementApplicationRelativePath); 
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