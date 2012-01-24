using System;
using System.IO;
using MbUnit.Framework;

namespace CassetteHelperTests
{
    [TestFixture]
    public class LineReplacerTests
    {
        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void IfFileDoesNotExistThrowsFileNotFoundException()
        {
            new LineReplacer("test.js");
        }

        [Test, ExpectedArgumentNullException("filePath")]
        [Row("")]
        [Row(null)]
        public void NullOrEmptyFilePathThrowsArgumentNullException(string filePath)
        {
            new LineReplacer(filePath);
        }
    }

    public class LineReplacer
    {
        public LineReplacer(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");
            if(!File.Exists(filePath)) throw new FileNotFoundException("Could not find file to replace lines in", filePath);
        }
    }
}
