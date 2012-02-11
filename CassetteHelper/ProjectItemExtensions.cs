using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;

namespace CassetteHelper
{
    public static class ProjectItemExtensions
    {
        public static IEnumerable<ProjectItem> Flatten(this IEnumerable<ProjectItem> projectItems)
        {
            foreach (var projectItem in projectItems)
            {
                var children = projectItem.ProjectItems.Cast<ProjectItem>();
                foreach (var child in children.Flatten())
                {
                    yield return child;
                }

                yield return projectItem;
            }
        }

        public static IEnumerable<ProjectItem> WithExtension(this IEnumerable<ProjectItem> projectItems, string fileExtension)
        {
            fileExtension = fileExtension.ToLowerInvariant();

            foreach (var projectItem in projectItems)
            {
                for (short i = 0; i < projectItem.FileCount; i++)
                {
                    var targetFileName = projectItem.FileNames[i];
                    if (Path.GetExtension(targetFileName).ToLowerInvariant() == fileExtension)
                    {
                        yield return projectItem;
                    }
                }
            }
        }
    }
}