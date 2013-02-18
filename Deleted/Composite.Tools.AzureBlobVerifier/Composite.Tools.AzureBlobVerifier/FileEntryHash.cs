using System;
using System.Linq;

namespace Composite.Tools.AzureBlobVerifier
{
	internal class FileEntryHash
	{
		public byte[] HashBytes { get; set; }

		public override string ToString()
		{
			return this.HashBytes == null ? "0" : BitConverter.ToString(this.HashBytes).Replace("-", "");
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as FileEntryHash);
		}

		public bool Equals(FileEntryHash fileEntryHash)
		{
			if (fileEntryHash == null) return false;

			if ((fileEntryHash.HashBytes != null) && (this.HashBytes != null))
			{
				return fileEntryHash.HashBytes.SequenceEqual(this.HashBytes);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return this.HashBytes.ComputeHash();
		}
	}
}
