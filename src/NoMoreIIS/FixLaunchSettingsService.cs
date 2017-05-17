using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;

namespace NoMoreIIS
{
    internal sealed class FixLaunchSettingsService
    {
        [ContextStatic]
        private static CommandEvents _commandEvents;

        private DTE _dte;
        private HashSet<string> _commandsToProxy = new HashSet<string>()
        {
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}882", // build all
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}295", // start debugging
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}368" // start without debugging
        };

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
            Debug.WriteLine($"CommandID={commandId} Id={id}");
            return _commandsToProxy.Contains($"{commandId}{id}");
        }

        private void FixLaunchSettings()
        {
            IEnumerable<string> filesToFix = GetLaunchSettingsFiles();

            //MessageBox.Show("FIXING");
        }

        private IList<string> GetLaunchSettingsFiles()
        {
            IEnumerable<Project> projects = GetSolutionProjects();
            var result = new List<string>();

            foreach (var project in projects)
            {
                if (string.Equals(project.Kind, Definitions.SolutionProjects.NetCoreProject, StringComparison.OrdinalIgnoreCase))
                {
                    string expectedLaunchSettingsFile = string.Join(Path.DirectorySeparatorChar.ToString(), Directory.GetParent(project.FullName), Definitions.LaunchSettingsPath);

                    if (File.Exists(expectedLaunchSettingsFile))
                    {
                        Debug.WriteLine($"I'm DOT NET CORE PROJECT WITH LAUNCH SETTINGS: {project.FullName}{Environment.NewLine}{expectedLaunchSettingsFile}");
                        result.Add(expectedLaunchSettingsFile);
                    }
                }
            }

            return result;
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

            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
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
