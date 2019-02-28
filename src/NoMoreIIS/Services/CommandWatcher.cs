using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using NoMoreIIS.Models;

namespace NoMoreIIS.Services
{
    internal sealed class CommandWatcher
    {
        [ContextStatic]
        private static CommandEvents _commandEvents;

        private DTE _dte;
        private readonly IFileSystemService _fileSystemService;
        private readonly IFixJsonLaunchSettingsService _fixJsonService;

        private HashSet<string> _commandsToProxy = new HashSet<string>()
        {
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}882", // build all
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}295", // start debugging
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}368" // start without debugging
        };

        public CommandWatcher(IFileSystemService fileSystemService = null, IFixJsonLaunchSettingsService fixJsonService = null)
        {
            _fileSystemService = fileSystemService ?? new FileSystemService();
            _fixJsonService = fixJsonService ?? new FixJsonLaunchSettingsService();
        }

        public void Initialize(DTE dte)
        {
            _dte = dte;
            _commandEvents = _dte.Events.CommandEvents;
            _commandEvents.BeforeExecute += CommandEvents_BeforeExecute;
        }

        private void CommandEvents_BeforeExecute(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            if (IsNeedToFixLaunchSettings(guid, id))
            {
                FixLaunchSettings();
            }
        }

        private bool IsNeedToFixLaunchSettings(string commandId, int id)
        {
            return _commandsToProxy.Contains($"{commandId}{id}");
        }

        private void FixLaunchSettings()
        {
            IList<LaunchSettingsMetadata> metadata = GetLaunchSettingsMetadata();

            _fixJsonService.Fix(metadata);
        }

        private IList<LaunchSettingsMetadata> GetLaunchSettingsMetadata()
        {
            return (from project in GetSolutionProjects()
                    where Definitions.SolutionProjects.NetCoreProjectKinds.Any(f => string.Equals(project.Kind, f, StringComparison.OrdinalIgnoreCase))
                    let expectedLaunchSettingsFile = string.Join(Path.DirectorySeparatorChar.ToString(), Directory.GetParent(project.FullName), Definitions.LaunchSettingsFile.Path)
                    where _fileSystemService.FileExists(expectedLaunchSettingsFile)
                    select new LaunchSettingsMetadata(project.Name, expectedLaunchSettingsFile))
                    .ToList();
        }

        private IEnumerable<Project> GetSolutionProjects()
        {
            var projects = _dte.Solution.Projects;
            var result = new List<Project>();
            IEnumerator item = projects.GetEnumerator();
            while (item.MoveNext())
            {
                var project = item.Current as Project;
                if (project == null)
                {
                    continue;
                }

                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    result.AddRange(GetSolutionFolderProjects(project));
                }
                else
                {
                    result.Add(project);
                }
            }

            return result;
        }

        private IEnumerable<Project> GetSolutionFolderProjects(Project solutionFolder)
        {
            var result = new List<Project>();

            for (int i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            {
                var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }

                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    result.AddRange(GetSolutionFolderProjects(subProject));
                }
                else
                {
                    result.Add(subProject);
                }
            }

            return result;
        }
    }
}
