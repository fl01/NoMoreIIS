using System.Collections.Generic;
using NoMoreIIS.Models;

namespace NoMoreIIS.Services
{
    public interface IFixJsonLaunchSettingsService
    {
        void Fix(IEnumerable<LaunchSettingsMetadata> metadata);
    }
}
