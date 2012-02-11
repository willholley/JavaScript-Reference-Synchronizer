﻿using System;
using System.IO;

namespace CassetteHelper
{
    // TODO: this should really just work with a stream to make testing easier
    public class FileContentReplacer
    {
        private readonly IReplacementStrategy replacementStrategy;

        public FileContentReplacer(IReplacementStrategy replacementStrategy)
        {
            this.replacementStrategy = replacementStrategy;
        }
        
        public void Replace(string targetFilePath)
        {
            if (string.IsNullOrEmpty(targetFilePath)) throw new ArgumentNullException("targetFilePath");
            if (!File.Exists(targetFilePath)) throw new FileNotFoundException("Could not find file to replace lines in", targetFilePath);
            
            using (var stringWriter = new StringWriter())
            {
                bool containsReferences = false;

                using (var inputStream = File.OpenText(targetFilePath))
                {
                    string line;

                    while ((line = inputStream.ReadLine()) != null)
                    {
                        var replacement = replacementStrategy.Replace(line);
                        containsReferences = containsReferences || (line != replacement);

                        stringWriter.WriteLine(replacement);
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