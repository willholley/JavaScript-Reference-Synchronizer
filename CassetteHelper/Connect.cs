using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
using EnvDTE80;
namespace CassetteHelper
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2
	{
		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;

		    try
            {
                events = _applicationObject.Events as Events2;
                projectItemsEvents = events.ProjectItemsEvents;
                projectItemsEvents.ItemRenamed += ProjectItemRenamed;
		    }
		    catch (Exception ex)
		    {
		        MessageBox.Show(ex.ToString());
		    }
		}
        
	    /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

	    private void ProjectItemRenamed(ProjectItem projectitem, string oldName)
	    {
            try
            {
                var newFullPath = projectitem.FileNames[0];

                var oldFullPath = Path.Combine(GetContainingDirectory(newFullPath), oldName);
                var baseDirectory = GetContainingDirectory(projectitem.ContainingProject.FileName);

                var replacementStrategy = new ApplicationRelativeReplacementStrategy(baseDirectory, oldFullPath, newFullPath);
                var streamReplacer = new FileContentReplacer(replacementStrategy);

                var projectItems = projectitem.ContainingProject.ProjectItems.Cast<ProjectItem>();
                var jsFiles = projectItems.Flatten().GetFilesWithExtension(".js");

                //Parallel.ForEach(jsFiles, streamReplacer.Replace);
                foreach (var file in jsFiles)
                {
                    streamReplacer.Replace(file);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
	    }

	    private string GetContainingDirectory(string filePath)
	    {
	        return new FileInfo(filePath).DirectoryName;
	    }

	    /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		private DTE2 _applicationObject;
		private AddIn _addInInstance;
	    private Events2 events;
	    private ProjectItemsEvents projectItemsEvents;
	}
}