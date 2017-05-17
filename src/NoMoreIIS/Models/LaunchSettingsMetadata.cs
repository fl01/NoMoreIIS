namespace NoMoreIIS.Models
{
    public class LaunchSettingsMetadata
    {
        public string ProjectName { get; set; }

        public string Location { get; set; }

        public LaunchSettingsMetadata(string projectName, string location)
        {
            ProjectName = projectName;
            Location = location;
        }
    }
}
