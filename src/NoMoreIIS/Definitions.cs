using System.Collections.Generic;

namespace NoMoreIIS
{
    public static class Definitions
    {
        public static class SolutionProjects
        {
            public static List<string> NetCoreProjectKinds = new List<string>() { "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}", "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}" };
        }

        public static class LaunchSettingsFile
        {
            public const string Path = @"Properties\launchSettings.json";
            public const string ProfilesNodeName = "profiles";
            public const string IISProfileNodeName = "IIS Express";
            public const string LaunchBrowserFieldName = "launchBrowser";
        }
    }
}
