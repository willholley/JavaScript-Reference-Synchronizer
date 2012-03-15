using System;
using System.Collections.Generic;
using System.IO;
using CassetteHelper.Replacement;

namespace CassetteHelper
{
    // TODO: this should really just work with a stream to make testing easier
    public class FileContentReplacer
    {
        public const string DELETED = "!!DELETED!!12345";

        private readonly IEnumerable<IReplacementStrategy> replacementStrategies;

        public FileContentReplacer(params IReplacementStrategy[] replacementStrategies)
        {
            this.replacementStrategies = replacementStrategies;
        }

        public void Replace(string targetFilePath)
        {
            if (string.IsNullOrEmpty(targetFilePath)) throw new ArgumentNullException("targetFilePath");
            if (!File.Exists(targetFilePath)) throw new FileNotFoundException("Could not find file to replace lines in", targetFilePath);

            using (var stringWriter = new StringWriter())
            {
                bool containsReferences = false;

                foreach (var replacementStrategy in replacementStrategies)
                {
                    replacementStrategy.SetContext(targetFilePath);
                }

                using (var inputStream = File.OpenText(targetFilePath))
                {
                    string line;

                    while ((line = inputStream.ReadLine()) != null)
                    {
                        string replacement = null;
                        foreach (var replacementStrategy in replacementStrategies)
                        {
                            replacement = replacementStrategy.Replace(line);

                            if (line != replacement) break;
                        }

                        containsReferences = containsReferences || (line != replacement);

                        // yuk
                        if (replacement != DELETED)
                        {
                            stringWriter.WriteLine(replacement);
                        }
                    }
                }

                if (containsReferences)
                {
                    File.WriteAllText(targetFilePath, stringWriter.ToString());
                }
            }
        }
    }
}