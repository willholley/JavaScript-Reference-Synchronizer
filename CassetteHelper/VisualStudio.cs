using EnvDTE80;

namespace CassetteHelper
{
    public interface IVisualStudio
    {
        void SaveAllDocuments();
        void OpenFile(string filePath);
    }

    public class VisualStudio : IVisualStudio
    {
        private readonly DTE2 applicationInstance;

        public VisualStudio(DTE2 applicationInstance)
        {
            this.applicationInstance = applicationInstance;
        }

        public void SaveAllDocuments()
        {
            applicationInstance.Documents.SaveAll();
        }

        //public bool IsOpenFile(string fileName)
        //{
        //    applicationInstance.Documents.
        //}

        public void OpenFile(string filePath)
        {
            applicationInstance.Documents.Open(filePath);
        }
    }
}