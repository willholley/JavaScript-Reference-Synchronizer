using System.IO;

namespace CassetteHelper.Matching
{
    public interface IReplacementStrategy
    {
        string Replace(string line);
    }
}