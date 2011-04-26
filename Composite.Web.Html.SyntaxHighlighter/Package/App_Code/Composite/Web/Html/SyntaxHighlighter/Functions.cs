using System.IO;
using Composite.Core.IO;
using Composite.Data;
using Composite.Data.Types;
using Composite.Functions;

namespace Composite.Web.Html.SyntaxHighlighter
{
	public class Functions
	{
		[FunctionParameterDescription("relativePath", "Relative Path", "Example: App_Code/Composite/Web/Html/SyntaxHighlighter/Functions.cs")]
		public static string LoadFile(string relativePath)
		{
			string path = Path.Combine(PathUtil.Resolve("~"), relativePath);
			if (!C1File.Exists(path))
			{
				throw new FileNotFoundException("File not found. Ensure path is relative (that it does not start with '/').", path);
			}
			using (var streamReader = new C1StreamReader(path))
			{
				return streamReader.ReadToEnd();
			}
		}

		[FunctionParameterDescription("mediaFileReference", " Media File Reference", "A file in the Media Archive with source code")]
		public static string LoadFileFromMedia(DataReference<IMediaFile> mediaFileReference)
		{
			return mediaFileReference.Data.ReadAllText();
		}
	}
}