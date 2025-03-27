// ReSharper disable InconsistentNaming
namespace UnityDownloader;

public sealed class VersionRecord
{
    public string SnapshotAt { get; set; }
    public string WebMajorVersionText { get; set; }

    public Dictionary<string, VersionRecordItem> Versions { get; set; } = new Dictionary<string, VersionRecordItem>();

    public class VersionRecordItem
    {
        public string releaseDate { get; set; }
        public string hash { get; set; }
        public string version { get; set; }
        public string linux { get; set; }
        public string mac { get; set; }
        public string macArm64 { get; set; }
        public string win64 { get; set; }
    }
}