

namespace Composite.Tools.AzureBlobVerifier
{
    public class FileChangeEntry
    {
        public string Path { get; set; }
        public FileChangeEntryType Type { get; set; }

        public override string ToString()
        {
            return this.Type + ": " + this.Path;
        }
    }
}
