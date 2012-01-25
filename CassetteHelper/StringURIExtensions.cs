using System;

namespace CassetteHelper
{
    public static class StringURIExtensions
    {
        public static string URIRelativeTo(this string filePath, string baseFile)
        {
            return new Uri(baseFile).MakeRelativeUri(new Uri(filePath)).ToString();
        }
    }
}