using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.Core.IO;

namespace Composite.Tools.LegacyUrlHandler.BrokenLinks
{
	public static class Config
	{
		private static readonly XElement ConfigFile =
			XElement.Load(PathUtil.Resolve("~/App_Data/Composite.Tools.LegacyUrlHandler/Config.xml"));

		static Config()
		{
			if (ConfigFile == null) return;
			FromEmail = ConfigFile.Elements("FromEmail").Select(el => el.Attribute("value").Value).FirstOrDefault();
			SendEveryNHours = ConfigFile.Elements("SendEveryNHours").Select(el => el.Attribute("value").Value).FirstOrDefault();
			RecipientEmails = GetValuesBySettingsName("RecipientEmails");
			IPBlackList = GetValuesBySettingsName("IPBlackList");
			RefererBlackList = GetValuesBySettingsName("RefererBlackList");
			UserAgentBlackList = GetValuesBySettingsName("UserAgentBlackList");
		}

		public static string SendEveryNHours { get; set; }
		public static string FromEmail { get; set; }

		public static List<string> RecipientEmails { get; set; }

		public static List<string> IPBlackList { get; set; }

		public static List<string> RefererBlackList { get; set; }

		public static List<string> UserAgentBlackList { get; set; }

		public static List<string> GetValuesBySettingsName(string name)
		{
			return ConfigFile.Elements(name).Descendants("add").Select(el =>
																		{
																			var attrValue = el.Attribute("value");
																			return attrValue != null ? attrValue.Value : null;
																		}).ToList();
		}
	}
}