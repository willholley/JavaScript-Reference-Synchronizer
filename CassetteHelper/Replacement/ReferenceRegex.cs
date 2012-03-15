using System.Text.RegularExpressions;

namespace CassetteHelper.Replacement
{
    public static class ReferenceRegex
    {
        private const string FormatString = "^\\W*///\\W*<reference\\W+path=\"{0}\"\\W*/>";

        public static Regex ApplicationRelative(string applicationRootDirectory, string originalFilePath)
        {
            var applicationRelativePath = originalFilePath.URIRelativeTo(applicationRootDirectory);
            return new Regex(string.Format(FormatString, "~/" + applicationRelativePath), RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public static Regex FileRelative(string baseFilePath, string relativeFilePath)
        {
            var relativePath = relativeFilePath.URIRelativeTo(baseFilePath).Replace("/", "[\\\\/]");
            return new Regex("^\\W*///\\W*<reference\\W+path=\"" + relativePath + "\"\\W*/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}