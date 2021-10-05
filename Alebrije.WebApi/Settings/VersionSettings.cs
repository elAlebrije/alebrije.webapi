using System.Collections.Generic;

namespace Alebrije.WebApi.Settings
{
    public class VersionSettings
    {
        public string Name { get; set; }
        public IEnumerable<string> AllowedVersions { get; set; }
    }
}