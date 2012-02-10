using System;
using System.IO;

namespace CassetteHelper
{
    public static class StringURIExtensions
    {
        public static string URIRelativeTo(this string filePath, string baseFile)
        {
            baseFile = AssertTrailingSlash(baseFile);

            return new Uri(baseFile).MakeRelativeUri(new Uri(filePath)).ToString();
        }

        private static string AssertTrailingSlash(string baseFile)
        {
            if(Path.GetExtension(baseFile) == "" && !baseFile.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                baseFile += Path.DirectorySeparatorChar;
            }
            return baseFile;
        }
    }
}