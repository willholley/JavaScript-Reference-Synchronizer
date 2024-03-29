using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using JSReferenceSynchronizer.Replacement;

namespace JSReferenceSynchronizer
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
                        IncludeSubdirectories = true,
                        NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName
                    };

                    fileSystemWatcher.Renamed += ItemRenamed;
                    fileSystemWatcher.Created += ItemCreated;
                    fileSystemWatcher.Deleted += ItemDeleted;
                    
                    fileSystemWatcher.EnableRaisingEvents = true;

                    return fileSystemWatcher;
                })
                .ToList();
	    }

        private bool IsFileMove(string lastCreatedFileFullPath, string newFileFullPath)
        {
            var isFileDeletion = lastCreatedFileFullPath == null || File.GetCreationTimeUtc(lastCreatedFileFullPath) < DateTime.UtcNow.AddSeconds(-2);

            if (!isFileDeletion)
            {
                var lastCreatedFileName = new FileInfo(lastCreatedFileFullPath).Name;
                var deletedFileName = new FileInfo(newFileFullPath).Name;

                var isFileMove = lastCreatedFileName == deletedFileName;

                if (isFileMove)
                {
                    return true;
                }
            }

            return false;
        }

        private string lastCreatedFileFullPath;
	    private void ItemDeleted(object sender, FileSystemEventArgs e)
	    {
            var watcher = (FileSystemWatcher)sender;
            
            if (IsFileMove(lastCreatedFileFullPath, e.FullPath))
            {
                MoveReferences(watcher.Path, e.FullPath, lastCreatedFileFullPath);
	        }
            else
            {
                DeleteReferences(watcher.Path, e.FullPath);
            }

            lastCreatedFileFullPath = null;
	    }

	    private void ItemCreated(object sender, FileSystemEventArgs e)
	    {
	        lastCreatedFileFullPath = e.FullPath;
	    }

        private void ItemRenamed(object sender, RenamedEventArgs e)
        {
            var fileSystemWatcher = (FileSystemWatcher)sender;
            var baseDirectory = fileSystemWatcher.Path;
            var oldFullPath = e.OldFullPath;
            var newFullPath = e.FullPath;

            MoveReferences(baseDirectory, oldFullPath, newFullPath);
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


        private void DeleteReferences(string projectDirectory, string deletedFileFullPath)
        {
            var replacementStrategies = new IReplacementStrategy[] {
                new ApplicationRelativeDeletionStrategy(projectDirectory, deletedFileFullPath),
                new FileRelativeDeletionStrategy(deletedFileFullPath)
            };

            UpdateFiles(projectDirectory, replacementStrategies);
        }

	    private void MoveReferences(string projectDirectory, string oldFilePath, string newFilePath)
	    {
            var replacementStrategies = new IReplacementStrategy[] { 
                new ApplicationRelativeReplacementStrategy(projectDirectory, oldFilePath, newFilePath),
                new FileRelativeMatchingStrategy(oldFilePath, newFilePath)
            };
	        
            UpdateFiles(projectDirectory, replacementStrategies);
	    }

        private void UpdateFiles(string projectDirectory, IReplacementStrategy[] replacementStrategies)
        {
            try
            {
                var streamReplacer = new FileContentReplacer(replacementStrategies);
                
                var javascriptFiles = Directory.GetFiles(projectDirectory, "*.js", SearchOption.AllDirectories);

                SaveOpenJavaScriptFiles();

                //Parallel.ForEach(jsFiles, streamReplacer.Replace);
                foreach (var javascriptFile in javascriptFiles)
                {
                    streamReplacer.Replace(javascriptFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

	    private void SaveOpenJavaScriptFiles()
	    {
	        var openJavaScriptDocuments =
	            _applicationObject.Documents.Cast<Document>().Where(
	                d => Path.GetExtension(d.FullName).ToLowerInvariant() == ".js");

	        foreach (var document in openJavaScriptDocuments)
	        {
	            document.Save();
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
	    private SolutionEvents solutionEvents;
	    private IList<FileSystemWatcher> fileSystemWatchers;
	}
}