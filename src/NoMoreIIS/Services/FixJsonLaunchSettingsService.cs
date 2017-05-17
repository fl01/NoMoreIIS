using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
using NoMoreIIS.Models;

namespace NoMoreIIS.Services
{
    public class FixJsonLaunchSettingsService : IFixJsonLaunchSettingsService
    {
        private readonly IFileSystemService _fileSystemService;

        public FixJsonLaunchSettingsService(IFileSystemService fileSystemService = null)
        {
            _fileSystemService = fileSystemService ?? new FileSystemService();
        }

        public void Fix(IEnumerable<LaunchSettingsMetadata> metadata)
        {
            if (metadata == null || !metadata.Any())
            {
                return;
            }

            foreach (LaunchSettingsMetadata launchSettings in metadata)
            {
                FixSafely(launchSettings);
            }
        }

        private void FixSafely(LaunchSettingsMetadata metadata)
        {
            try
            {
                string rawJson = _fileSystemService.GetFileContent(metadata.Location);
                JToken settings = JToken.Parse(rawJson);

                bool removedIISProfile = RemoveIISProfile(settings);
                bool disabledBrowserLaunch = DisableBrowserLaunch(metadata, settings);
                if (removedIISProfile || disabledBrowserLaunch)
                {
                    _fileSystemService.WriteAllText(metadata.Location, settings.ToString());
                }
            }
            catch (Exception ex) // at this point we do not care about the type of exception
            {
                Debug.WriteLine($"Failed to fix file '{metadata.Location}'.{Environment.NewLine}{ex.ToString()}");
            }
        }

        private bool DisableBrowserLaunch(LaunchSettingsMetadata metadata, JToken root)
        {
            JProperty kestrelProfile = root?[Definitions.LaunchSettingsFile.ProfilesNodeName]?
                                .Children<JProperty>()
                                .FirstOrDefault(p => p.Name == metadata.ProjectName);

            if (kestrelProfile != null)
            {
                var launchBrowser = kestrelProfile.Value[Definitions.LaunchSettingsFile.LaunchBrowserFieldName];
                if (launchBrowser != null && launchBrowser.Type == JTokenType.Boolean && (bool)launchBrowser)
                {
                    kestrelProfile.Value[Definitions.LaunchSettingsFile.LaunchBrowserFieldName] = false;
                    return true;
                }
            }

            return false;
        }

        private bool RemoveIISProfile(JToken root)
        {
            JProperty iisProfile = root?[Definitions.LaunchSettingsFile.ProfilesNodeName]?
                    .Children<JProperty>()
                    .FirstOrDefault(p => string.Equals(Definitions.LaunchSettingsFile.IISProfileNodeName, p.Name, StringComparison.OrdinalIgnoreCase));

            if (iisProfile != null)
            {
                iisProfile.Remove();
                return true;
            }

            return false;
        }
    }
}
