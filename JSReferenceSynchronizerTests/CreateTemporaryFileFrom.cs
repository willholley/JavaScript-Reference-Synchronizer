using System;
using System.IO;
using System.Reflection;

namespace JSReferenceSynchronizer.Tests
{
    public static class CreateTemporaryFileFrom
    {
        /// <summary>
        /// Helper method to write an embedded resource to a temporary file. The file created has a random filename
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static TemporaryFile EmbeddedResource(string resourceName)
        {
            string outPath = Path.GetTempFileName() + Path.GetExtension(resourceName);

            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("CassetteHelperTests.Resources." + resourceName))
            {
                using (FileStream output = File.Create(outPath))
                {
                    byte[] buffer = new byte[4096];
                    while (input.Position < input.Length)
                    {
                        int len = input.Read(buffer, 0, 4096);
                        output.Write(buffer, 0, len);
                    }
                }
            }

            return new TemporaryFile(outPath);
        }
    }


    public class TemporaryFile : IDisposable
    {
        public readonly string AbsolutePath;

        public TemporaryFile(string absolutePath)
        {
            this.AbsolutePath = absolutePath;
        }
        
        public override string ToString()
        {
            return AbsolutePath;
        }

        public void Dispose()
        {
            if (File.Exists(AbsolutePath))
            {
                File.Delete(AbsolutePath);
            }
        }
    }
}