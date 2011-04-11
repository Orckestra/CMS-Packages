using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Xml.Linq;
using Composite.Functions;

namespace Composite.Search.MicrosoftSearchServer
{
	public class Functions
	{
		public static IEnumerable<XElement> GetSearchResult(string SearchQuery, int PageSize, int PageCount, string SearchServiceUrl, string Scope, ICredentials Credentials)
		{
			var query = @"SELECT WorkId,Rank,Title,Author,Size,Path,Write,HitHighlightedSummary FROM Scope() WHERE freetext('" + SearchQuery.Replace('\'', ' ') + @"')";
			if (!string.IsNullOrEmpty(Scope))
			{
				query = query + string.Format(@"AND ""Scope""='{0}'", Scope);
			}

			XNamespace ns = "urn:Microsoft.Search.Query";
			XElement queryPacket = new XElement(ns + "QueryPacket",
				new XElement(ns + "Query",
					new XElement(ns + "Context",
						new XElement(ns + "QueryText",
							new XAttribute("language", CultureInfo.CurrentCulture.ToString()),
							new XAttribute("type", "MSSQLFT"),
								query
							)
						), new XElement(ns + "TrimDuplicates", false),
						new XElement(ns + "Range",
							new XElement(ns + "StartAt", (PageCount - 1) * PageSize + 1),
							new XElement(ns + "Count", PageSize)
						)
					)
				);

			QueryService queryService = new QueryService();
			queryService.Url = SearchServiceUrl;
			queryService.Credentials = Credentials;
			string queryResultsString = queryService.Query(queryPacket.ToString());

			return new List<XElement>() {
				XElement.Parse(CleanInvalidXmlChars(queryResultsString))
			};
		}

		[FunctionParameterDescription("Username", "Username", "The user name associated with the credentials")]
		[FunctionParameterDescription("Password", "Password", "The password for the user name associated with the credentials")]
		public static ICredentials GetNetworkCredential(string Username, string Password)
		{
			return new NetworkCredential(Username, Password);
		}

		public static ICredentials GetDefaultNetworkCredentials()
		{
			return CredentialCache.DefaultNetworkCredentials;
		}

		public static string CleanInvalidXmlChars(string text)
		{
			return text.Replace("&#xB;", "");

		}
	}
}
