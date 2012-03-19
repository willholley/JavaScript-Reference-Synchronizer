using System;
using System.Text.RegularExpressions;

namespace JSReferenceSynchronizer.Replacement
{
    public class FileRelativeDeletionStrategy : IReplacementStrategy
    {
        private readonly string originalFilePath;
        private Regex matcher;

        public FileRelativeDeletionStrategy(string originalFilePath)
        {
            if (String.IsNullOrEmpty(originalFilePath)) throw new ArgumentNullException("originalFilePath");

            this.originalFilePath = originalFilePath;
        }

        public void SetContext(string targetFileFullName)
        {
            this.matcher = ReferenceRegex.FileRelative(targetFileFullName, originalFilePath);
        }

        public string Replace(string line)
        {
            if (!string.IsNullOrEmpty(line) && matcher.IsMatch(line))
            {
                return FileContentReplacer.DELETED;
            }

            return line;
        }
    }
}