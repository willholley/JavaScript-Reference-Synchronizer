using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
                events.ProjectItemsEvents.ItemRenamed += ProjectItemRenamed;
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

        private string NormaliseTo(string baseDirectory, string filePath)
        {
            return filePath.Replace(baseDirectory, "").Replace(Path.DirectorySeparatorChar, '/');
        }

	    private void ProjectItemRenamed(ProjectItem projectitem, string oldName)
	    {
	        var newFullPath = projectitem.FileNames[0];
	        var oldFullPath = Path.Combine(Path.GetDirectoryName(newFullPath), oldName);
	        var baseDirectory = Path.GetDirectoryName(projectitem.ContainingProject.FileName);

	        var oldUrl = NormaliseTo(baseDirectory, oldFullPath);
            var newUrl = NormaliseTo(baseDirectory, newFullPath);

	        var projectItems = projectitem.ContainingProject.ProjectItems.Cast<ProjectItem>();
	        ForEachProjectItem(projectItems, pi => FindJavaScriptFiles(pi, newUrl, oldUrl));
	    }

	    private void FindJavaScriptFiles(ProjectItem projectItem, string newUrl, string oldUrl)
	    {
            //var applicationRelativeReference = new Regex("^/// \\<reference path=\"~/" + oldUrl + ")\" /\\>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
	    
            const string referenceFormatString = "/// <reference path=\"~/{0}\" />";
	        var oldReference = string.Format(referenceFormatString, oldUrl);
            var newReference = string.Format(referenceFormatString, newUrl);

	        for (short i = 0; i < projectItem.FileCount; i++)
	        {
	            var targetFileName = projectItem.FileNames[i];
                if (Path.GetExtension(targetFileName) == ".js")
                {
                    var targetText = File.ReadAllText(targetFileName);
                    var patchedText = targetText.Replace(oldReference, newReference);

                    if(targetText != patchedText)
                    {
                        File.WriteAllText(targetFileName, patchedText);
                    }
                }
            }
	    }

	    private static void ForEachProjectItem(IEnumerable<ProjectItem> projectItems, Action<ProjectItem> action)
	    {
	        foreach (var projectItem in projectItems)
	        {
	            ForEachProjectItem(projectItem.ProjectItems.Cast<ProjectItem>(), action);
	            action(projectItem);
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
	}
}