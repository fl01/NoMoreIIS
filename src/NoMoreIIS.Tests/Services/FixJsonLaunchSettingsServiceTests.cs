using Moq;
using Newtonsoft.Json.Linq;
using NoMoreIIS.Models;
using NoMoreIIS.Services;
using Xunit;

namespace NoMoreIIS.Tests.Services
{
    public class FixJsonLaunchSettingsServiceTests
    {
        private Mock<IFileSystemService> _fileSystemMock;

        public FixJsonLaunchSettingsServiceTests()
        {
            _fileSystemMock = new Mock<IFileSystemService>();
        }

        [Fact]
        public void Fix_JsonSettingsWithIISProfile_IISProfileIsRemoved()
        {
            var svc = Create();
            var metadata = new LaunchSettingsMetadata("bla bla", "somelocation");
            string original = "{\r\n  \"iisSettings\": {\r\n    \"windowsAuthentication\": false,\r\n    \"anonymousAuthentication\": true,\r\n    \"iisExpress\": {\r\n      \"applicationUrl\": \"http://localhost:50892/\",\r\n      \"sslPort\": 0\r\n    }\r\n  },\r\n  \"profiles\": {\r\n    \"IIS Express\": {\r\n      \"commandName\": \"IISExpress\",\r\n      \"launchBrowser\": true,\r\n      \"environmentVariables\": {\r\n        \"ASPNETCORE_ENVIRONMENT\": \"Development\"\r\n      }\r\n    },\r\n    \"WebApplication3\": {\r\n      \"commandName\": \"Project\",\r\n      \"launchUrl\": \"api/values\",\r\n      \"environmentVariables\": {\r\n        \"ASPNETCORE_ENVIRONMENT\": \"Development\"\r\n      },\r\n      \"applicationUrl\": \"http://localhost:50893\"\r\n    }\r\n  }\r\n}";
            string expected = "{\r\n  \"iisSettings\": {\r\n    \"windowsAuthentication\": false,\r\n    \"anonymousAuthentication\": true,\r\n    \"iisExpress\": {\r\n      \"applicationUrl\": \"http://localhost:50892/\",\r\n      \"sslPort\": 0\r\n    }\r\n  },\r\n  \"profiles\": {\r\n    \"WebApplication3\": {\r\n      \"commandName\": \"Project\",\r\n      \"launchUrl\": \"api/values\",\r\n      \"environmentVariables\": {\r\n        \"ASPNETCORE_ENVIRONMENT\": \"Development\"\r\n      },\r\n      \"applicationUrl\": \"http://localhost:50893\"\r\n    }\r\n  }\r\n}";
            _fileSystemMock.Setup(x => x.GetFileContent(metadata.Location)).Returns(original);

            svc.Fix(new[] { metadata });

            _fileSystemMock.Verify(x => x.WriteAllText(metadata.Location, expected), Times.Once);
        }

        [Fact]
        public void Fix_KestrelProfileWithEnabledBrowserLaunch_LaunchBrowserIsSetToTrue()
        {
            var svc = Create();
            var metadata = new LaunchSettingsMetadata("WebApplication3", "somelocation");
            string original = "{\r\n  \"iisSettings\": {\r\n    \"windowsAuthentication\": false,\r\n    \"anonymousAuthentication\": true,\r\n    \"iisExpress\": {\r\n      \"applicationUrl\": \"http://localhost:50892/\",\r\n      \"sslPort\": 0\r\n    }\r\n  },\r\n  \"profiles\": {\r\n    \"WebApplication3\": {\r\n      \"commandName\": \"Project\",\r\n      \"launchBrowser\": true,\r\n      \"launchUrl\": \"api/values\",\r\n      \"environmentVariables\": {\r\n        \"ASPNETCORE_ENVIRONMENT\": \"Development\"\r\n      },\r\n      \"applicationUrl\": \"http://localhost:50893\"\r\n    }\r\n  }\r\n}";
            string expected = "{\r\n  \"iisSettings\": {\r\n    \"windowsAuthentication\": false,\r\n    \"anonymousAuthentication\": true,\r\n    \"iisExpress\": {\r\n      \"applicationUrl\": \"http://localhost:50892/\",\r\n      \"sslPort\": 0\r\n    }\r\n  },\r\n  \"profiles\": {\r\n    \"WebApplication3\": {\r\n      \"commandName\": \"Project\",\r\n      \"launchBrowser\": false,\r\n      \"launchUrl\": \"api/values\",\r\n      \"environmentVariables\": {\r\n        \"ASPNETCORE_ENVIRONMENT\": \"Development\"\r\n      },\r\n      \"applicationUrl\": \"http://localhost:50893\"\r\n    }\r\n  }\r\n}";
            _fileSystemMock.Setup(x => x.GetFileContent(metadata.Location)).Returns(original);

            svc.Fix(new[] { metadata });

            _fileSystemMock.Verify(x => x.WriteAllText(metadata.Location, expected), Times.Once);
        }

        [Fact]
        public void Fix_IISProfileAndEnabledBrowserLaunchForKestrel_BothFeaturesAreFixed()
        {
            var svc = Create();
            var metadata = new LaunchSettingsMetadata("WebApplication3", "somelocation");
            string original = "{\r\n  \"iisSettings\": {\r\n    \"windowsAuthentication\": false,\r\n    \"anonymousAuthentication\": true,\r\n    \"iisExpress\": {\r\n      \"applicationUrl\": \"http://localhost:50892/\",\r\n      \"sslPort\": 0\r\n    }\r\n  },\r\n  \"profiles\": {\r\n    \"IIS Express\": {\r\n      \"commandName\": \"IISExpress\",\r\n      \"launchBrowser\": true,\r\n      \"environmentVariables\": {\r\n        \"ASPNETCORE_ENVIRONMENT\": \"Development\"\r\n      }\r\n    },\r\n    \"WebApplication3\": {\r\n      \"commandName\": \"Project\",\r\n      \"launchBrowser\": false,\r\n      \"launchUrl\": \"api/values\",\r\n      \"environmentVariables\": {\r\n        \"ASPNETCORE_ENVIRONMENT\": \"Development\"\r\n      },\r\n      \"applicationUrl\": \"http://localhost:50893\"\r\n    }\r\n  }\r\n}";
            string expected = "{\r\n  \"iisSettings\": {\r\n    \"windowsAuthentication\": false,\r\n    \"anonymousAuthentication\": true,\r\n    \"iisExpress\": {\r\n      \"applicationUrl\": \"http://localhost:50892/\",\r\n      \"sslPort\": 0\r\n    }\r\n  },\r\n  \"profiles\": {\r\n    \"WebApplication3\": {\r\n      \"commandName\": \"Project\",\r\n      \"launchBrowser\": false,\r\n      \"launchUrl\": \"api/values\",\r\n      \"environmentVariables\": {\r\n        \"ASPNETCORE_ENVIRONMENT\": \"Development\"\r\n      },\r\n      \"applicationUrl\": \"http://localhost:50893\"\r\n    }\r\n  }\r\n}";
            _fileSystemMock.Setup(x => x.GetFileContent(metadata.Location)).Returns(original);

            svc.Fix(new[] { metadata });

            _fileSystemMock.Verify(x => x.WriteAllText(metadata.Location, expected), Times.Once);
        }

        private FixJsonLaunchSettingsService Create()
        {
            return new FixJsonLaunchSettingsService(_fileSystemMock.Object);
        }
    }
}
