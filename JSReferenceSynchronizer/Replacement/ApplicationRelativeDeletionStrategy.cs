using System.Text.RegularExpressions;

namespace JSReferenceSynchronizer.Replacement
{
    public class ApplicationRelativeDeletionStrategy : IReplacementStrategy
    {
        private readonly Regex matcher;

        public ApplicationRelativeDeletionStrategy(string applicationRootDirectory, string originalFilePath)
        {
            this.matcher = ReferenceRegex.ApplicationRelative(applicationRootDirectory, originalFilePath);
        }

        public void SetContext(string targetFileFullName)
        {
            // no op
        }

        public string Replace(string line)
        {
            if (matcher.IsMatch(line))
            {
                return FileContentReplacer.DELETED;
            }

            return line;
        }
    }
}