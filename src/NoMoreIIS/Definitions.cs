namespace NoMoreIIS
{
    public static class Definitions
    {
        public const string LaunchSettingsPath = @"Properties\launchSettings.json";

        public static class SolutionProjects
        {
            public const string NetCoreProjectKind = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";
        }

        public static class LaunchSettingsFile
        {
            public const string ProfilesNodeName = "profiles";
            public const string IISProfileNodeName = "IIS Express";
            public const string LaunchBrowserFieldName = "launchBrowser";
        }
    }
}
