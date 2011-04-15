using System;


namespace Composite.Tools.AzureBlobVerifier
{
    internal class FileEntry : IComparable
    {
        public string Path { get; set; }
        public FileEntryHash Hash { get; set; }


        public override string ToString()
        {
            return this.Path + " Hash: " + this.Hash.ToString();
        }



        public int CompareTo(object obj)
        {
            FileEntry fileEntry = obj as FileEntry;

            if (fileEntry == null) return -1;

            return fileEntry.Path.CompareTo(this.Path);
        }



        public override bool Equals(object obj)
        {
            return Equals(obj as FileEntry);
        }



        public bool Equals(FileEntry fileEntry)
        {
            if (fileEntry == null) return false;

            return fileEntry.Path == this.Path && Equals(fileEntry.Hash, this.Hash);
        }



        public override int GetHashCode()
        {
            return this.Path.GetHashCode() ^ this.Hash.GetHashCode();
        }
    }
}
