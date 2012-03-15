using System.IO;

namespace CassetteHelper.Matching
{
    public class ApplicationRelativeDeletionStrategy : IReplacementStrategy
    {
        private readonly ApplicationRelativeMatcher matcher;

        public ApplicationRelativeDeletionStrategy(string applicationRootDirectory, string originalFilePath)
        {
            this.matcher = new ApplicationRelativeMatcher(applicationRootDirectory, originalFilePath);
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