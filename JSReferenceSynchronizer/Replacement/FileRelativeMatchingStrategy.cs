using System;
using System.Text.RegularExpressions;

namespace JSReferenceSynchronizer.Replacement
{
    public class FileRelativeMatchingStrategy : IReplacementStrategy
    {
        private readonly string originalFilePath;
        private readonly string newFilePath;
        private Regex matcher;
        private string targetFileFullName;

        public FileRelativeMatchingStrategy(string originalFilePath, string newFilePath)
        {
            if (String.IsNullOrEmpty(originalFilePath)) throw new ArgumentNullException("originalFilePath");
            if (String.IsNullOrEmpty(originalFilePath)) throw new ArgumentNullException("newFilePath");

            this.originalFilePath = originalFilePath;
            this.newFilePath = newFilePath;
        }

        public void SetContext(string targetFileFullName)
        {
            this.targetFileFullName = targetFileFullName;

            this.matcher = ReferenceRegex.FileRelative(targetFileFullName, originalFilePath);
        }

        public string Replace(string line)
        {
            if(!string.IsNullOrEmpty(line) && matcher.IsMatch(line))
            {
                return string.Format("/// <reference path=\"{0}\" />", newFilePath.URIRelativeTo(targetFileFullName)); 
            }

            return line;
        }
    }
}