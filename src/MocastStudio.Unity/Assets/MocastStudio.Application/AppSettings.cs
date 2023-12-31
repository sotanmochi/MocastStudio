using System;

namespace MocastStudio.Application
{
    [Serializable]
    public sealed class AppSettings
    {
        public string AppName { get; set; }
        public string AppVersion { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Ulid ClientId { get; internal set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool IsUpdated { get; set; }
    }
}
