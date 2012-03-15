using System;
using System.Collections.Generic;
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
                solutionEvents = events.SolutionEvents;
                solutionEvents.Opened += StartWatchingProjectDirectories;
                solutionEvents.BeforeClosing += StopWatchingProjectDirectories;

                this.visualStudio = new VisualStudio(_applicationObject);

                if (_applicationObject.Solution != null)
                {
                    StartWatchingProjectDirectories();
                }
            }
		    catch (Exception ex)
		    {
		        MessageBox.Show(ex.ToString());
		    }
		}

	    private void StopWatchingProjectDirectories()
	    {
            if (fileSystemWatchers == null) return;

	        foreach (var fileSystemWatcher in fileSystemWatchers)
	        {
                fileSystemWatcher.EnableRaisingEvents = false;
	        }

	        fileSystemWatchers = null;
	    }

	    private void StartWatchingProjectDirectories()
	    {
	        fileSystemWatchers = _applicationObject.Solution.Projects
                .Cast<Project>()
                .Where(p => !String.IsNullOrEmpty(p.FullName))
                .Select(p => {
                    var directory = new FileInfo(p.FullName).DirectoryName;

                    var fileSystemWatcher = new FileSystemWatcher(directory, "*.js")
                    {
                        IncludeSubdirectories = true
                    };

                    fileSystemWatcher.Renamed += ItemRenamed;
                    fileSystemWatcher.EnableRaisingEvents = true;

                    return fileSystemWatcher;
                })
                .ToList();
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



        private void ItemRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                var fileSystemWatcher = (FileSystemWatcher)sender;
                var baseDirectory = fileSystemWatcher.Path;

                var replacementStrategy = new ApplicationRelativeReplacementStrategy(baseDirectory, e.OldFullPath, e.FullPath);
                var streamReplacer = new FileContentReplacer(replacementStrategy);

                var javascriptFiles = Directory.GetFiles(baseDirectory, "*.js", SearchOption.AllDirectories);

                // TODO: save only JS files
                _applicationObject.Documents.SaveAll();
                    
                //Parallel.ForEach(jsFiles, streamReplacer.Replace);
                foreach (var projectItem in javascriptFiles)
                {
                    streamReplacer.Replace(projectItem);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
	    private VisualStudio visualStudio;
	    private SolutionEvents solutionEvents;
	    private IList<FileSystemWatcher> fileSystemWatchers;
	}
}