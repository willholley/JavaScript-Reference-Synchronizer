using System;
using System.IO;

namespace CassetteHelper
{
    public class LineReplacer
    {
        private readonly string newReference;
        private readonly ILineVisitor visitor;

        public LineReplacer(ILineVisitor visitor, string replacementUrl)
        {
            this.visitor = visitor;
            this.newReference = string.Format("/// <reference path=\"{0}\" />", replacementUrl); 
        }
        
        public void Replace(string targetFilePath)
        {
            if (string.IsNullOrEmpty(targetFilePath)) throw new ArgumentNullException("targetFilePath");
            if (!File.Exists(targetFilePath)) throw new FileNotFoundException("Could not find file to replace lines in", targetFilePath);
            
            using (var stringWriter = new StringWriter())
            {
                bool containsReferences;

                using (var inputStream = File.OpenText(targetFilePath))
                {
                    containsReferences = visitor.Visit(inputStream, 
                                                       r => stringWriter.WriteLine(this.newReference),
                                                       stringWriter.WriteLine);
                }

                if (containsReferences)
                {
                    File.WriteAllText(targetFilePath, stringWriter.ToString());
                }
            }
        }
    }
}