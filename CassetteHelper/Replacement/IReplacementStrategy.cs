namespace CassetteHelper.Replacement
{
    public interface IReplacementStrategy
    {
        void SetContext(string targetFileFullName);
        string Replace(string line);
    }
}