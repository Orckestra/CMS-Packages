using System;
using System.Web;
using System.Xml.Linq;

namespace Composite.Tools.AzureBlobVerifier
{
	public static class HtmlPresenter
	{
		public static XElement GetHtml(string repairUrl, string startPath = "/")
		{
			int t1 = Environment.TickCount;
			ValiddationResult result = Validator.Validate(startPath);
			int t2 = Environment.TickCount;

			XElement body = new XElement("div");

			body.Add(
				new XElement("div", new XAttribute("style", "padding: 6px;"),
					new XElement("h3", "Information"),
					new XElement("div", string.Format("Number of files validated: {0}", result.FilesValidated)),
					new XElement("div", string.Format("Validation completed in: {0} ms", TimeSpan.FromMilliseconds(t2 - t1).ToString("g")))
				)
			);

			AddOutOfSyncHtml(repairUrl, startPath, result, body);

			XElement ignoredTable = new XElement("table", new XAttribute("style", "empty-cells: show; padding: 10px; margin: 10px; border-collapse: collapse; border: 1px solid black;"));

			ignoredTable.Add(new XElement("tr",
				new XElement("th", new XAttribute("style", "vertical-align: top; border: 1px solid black; padding: 6px; margin: 0px; font-size: 110%;"), "File path")
			));

			foreach (string path in result.PathsThatAreIgnored)
			{
				ignoredTable.Add(new XElement("tr",
					new XAttribute("style", "padding: 0px; margin: 0px; border: 1px solid black;"),
					new XElement("td", new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;"), path)
				));
			}

			body.Add(
				new XElement("div", new XAttribute("style", "padding: 6px;"),
					new XElement("h3", "Files and directories that are getting ignored by C1 when writing through Composite.IO"),
					ignoredTable
				)
			);

			AddIgnoredFilesHtml(result, body);
			return body;
		}

		private static void AddOutOfSyncHtml(string repairUrl, string startPath, ValiddationResult result, XElement body)
		{
			if (result.FileEntries.Count > 0)
			{
				XElement outOfSyncTable = new XElement("table", new XAttribute("style", "empty-cells: show; padding: 10px; margin: 10px; border-collapse: collapse; border: 1px solid black;"));

				XElement header = new XElement("tr",
					new XElement("th", new XAttribute("style", "vertical-align: top; border: 1px solid black; padding: 6px; margin: 0px; font-size: 110%;"), "Type"),
					new XElement("th", new XAttribute("style", "vertical-align: top; border: 1px solid black; padding: 6px; margin: 0px; font-size: 110%;"), "File path"),
					new XElement("th", new XAttribute("style", "vertical-align: top; border: 1px solid black; padding: 6px; margin: 0px; font-size: 110%;"), "Repair")
				);

				outOfSyncTable.Add(header);

				foreach (FileChangeEntry entry in result.FileEntries)
				{
					XElement row = new XElement("tr",
						new XAttribute("style", "padding: 0px; margin: 0px; border: 1px solid black;"),
						new XElement("td", new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;"), entry.Type.ToPrettyString()),
						new XElement("td", new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;"), entry.Path)
						);

					if (entry.Type == FileChangeEntryType.MissingLocally)
					{
						row.Add(
							new XElement("td",
								new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;"),
								new XElement("a",
									new XAttribute("style", "color: #0000ff; cursor: pointer"),
									new XAttribute("href", repairUrl + "?Repair=" + FileChangeEntryType.MissingLocally + "&Path=" + HttpUtility.UrlEncode(entry.Path)),
									new XAttribute("title", "Copy blob version to local file storage"),
									"Copy blob to local")
								)
							);
					}
					else if (entry.Type == FileChangeEntryType.MissingInBlob)
					{
						row.Add(
							new XElement("td",
								new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;"),
								new XElement("a",
									new XAttribute("style", "color: #0000ff; cursor: pointer"),
									new XAttribute("href", repairUrl + "?Repair=" + FileChangeEntryType.MissingInBlob + "&Path=" + HttpUtility.UrlEncode(entry.Path)),
									new XAttribute("title", "Copy local file version to blob"),
									"Copy local to blob")
								)
							);
					}
					else if (entry.Type == FileChangeEntryType.Changed)
					{
						row.Add(
							new XElement("td",
								new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;"),
								new XElement("a",
									new XAttribute("style", "color: #0000ff; cursor: pointer"),
									new XAttribute("href", repairUrl + "?Repair=" + FileChangeEntryType.Changed + "Local" + "&Path=" + HttpUtility.UrlEncode(entry.Path)),
									new XAttribute("title", "Copy local file version to blob"),
									"Use local version"),
								new XElement("br"),
								new XElement("a",
									new XAttribute("style", "color: #0000ff; cursor: pointer"),
									new XAttribute("href", repairUrl + "?Repair=" + FileChangeEntryType.Changed + "Blob" + "&Path=" + HttpUtility.UrlEncode(entry.Path)),
									new XAttribute("title", "Copy blob version to local file storage"),
									"Use blob version")
								)
							);
					}

					outOfSyncTable.Add(row);
				}

				XElement[] finalRows = new XElement[] {
					new XElement("tr",
						new XAttribute("style", "padding: 0px; margin: 0px; border: 1px solid black;"),
						new XElement("td", new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;")),
						new XElement("td", new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;")),
						new XElement("td",
							new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;"),
							new XElement("a",
								new XAttribute("style", "color: #0000ff; cursor: pointer"),
								new XAttribute("href", repairUrl + "?Repair=AllLocal&Path=" + startPath),
								new XAttribute("title", "Repair all"),
								new XElement("b", "Repair all (Prefer local files)")
							)
						)
					),
					new XElement("tr",
						new XAttribute("style", "padding: 0px; margin: 0px; border: 1px solid black;"),
						new XElement("td", new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;")),
						new XElement("td", new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;")),
						new XElement("td",
							new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;"),
							new XElement("a",
								new XAttribute("style", "color: #0000ff; cursor: pointer"),
								new XAttribute("href", repairUrl + "?Repair=AllBlob&Path=" + startPath),
								new XAttribute("title", "Repair all"),
								new XElement("b", "Repair all (Prefer blob files)")
							)
						)
					)
				};

				outOfSyncTable.Add(finalRows);

				body.Add(
					new XElement("div", new XAttribute("style", "padding: 6px;"),
						new XElement("h3", "Files out of sync"),
						outOfSyncTable
					));
			}
			else
			{
				body.Add(
					new XElement("div", new XAttribute("style", "padding: 6px;"),
						new XElement("h3", "Files out of sync"),
						new XElement("div", "None!")
					)
				);
			}
		}

		private static void AddIgnoredFilesHtml(ValiddationResult result, XElement body)
		{
			XElement ignoredTable = new XElement("table", new XAttribute("style", "empty-cells: show; padding: 10px; margin: 10px; border-collapse: collapse; border: 1px solid black;"));

			ignoredTable.Add(new XElement("tr",
				new XElement("th", new XAttribute("style", "vertical-align: top; border: 1px solid black; padding: 6px; margin: 0px; font-size: 110%;"), "File path")
			));

			foreach (string path in result.IgnoredFiles)
			{
				ignoredTable.Add(new XElement("tr",
					new XAttribute("style", "padding: 0px; margin: 0px; border: 1px solid black;"),
					new XElement("td", new XAttribute("style", "background-color: #ffffff; vertical-align: top; border: 1px solid black; padding: 2px; margin: 0px;"), path)
				));
			}

			XElement ignoredElement =
				new XElement("div",
						new XElement("div", new XAttribute("onclick", "if (document.getElementById('ignoredResults').style.display == 'none') document.getElementById('ignoredResults').style.display = 'block'; else document.getElementById('ignoredResults').style.display = 'none';"), "Show/Hide"),
						new XElement("div", new XAttribute("id", "ignoredResults"), new XAttribute("style", "display: none;"), ignoredTable));

			body.Add(
				new XElement("div", new XAttribute("style", "padding: 6px;"),
					new XElement("h3", "Ignored files in the out of sync list"),
					ignoredElement
				)
			);
		}
	}
}
