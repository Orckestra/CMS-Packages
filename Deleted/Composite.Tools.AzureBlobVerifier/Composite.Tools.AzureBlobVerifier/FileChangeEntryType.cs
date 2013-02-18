
namespace Composite.Tools.AzureBlobVerifier
{
	public enum FileChangeEntryType
	{
		MissingLocally = 0,
		MissingInBlob = 1,
		Changed = 2,
	}

	public static class FileChangeEntryTypeExtensionMethods
	{
		public static string ToPrettyString(this FileChangeEntryType fileChangeEntryType)
		{
			switch (fileChangeEntryType)
			{
				case FileChangeEntryType.MissingLocally:
					return "Missing (Local)";

				case FileChangeEntryType.MissingInBlob:
					return "Missing (Blob)";

				case FileChangeEntryType.Changed:
					return "Changed";

				default:
					return fileChangeEntryType.ToString();
			}
		}
	}
}
