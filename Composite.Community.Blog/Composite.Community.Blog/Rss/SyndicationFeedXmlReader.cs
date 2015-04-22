using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Composite.Community.Blog.Rss
{
	public class SyndicationFeedXmlReader : XmlTextReader
	{
		readonly string[] _rss20DateTimeHints = { "pubDate", "lastBuildDate" };
		readonly string[] _atom10DateTimeHints = { "updated", "published" };
		private bool _isRss2DateTime = false;
		private bool _isAtomDateTime = false;

		public SyndicationFeedXmlReader(Stream stream) : base(stream) { }

		public override bool IsStartElement(string localname, string ns)
		{
			_isRss2DateTime = false;
			_isAtomDateTime = false;

			if (_rss20DateTimeHints.Contains(localname)) _isRss2DateTime = true;
			if (_atom10DateTimeHints.Contains(localname)) _isAtomDateTime = true;

			return base.IsStartElement(localname, ns);
		}

		public override string ReadString()
		{
			string dateVal = base.ReadString();

			try
			{
				if (_isRss2DateTime)
				{
					var objMethod = typeof(Rss20FeedFormatter).GetMethod("DateFromString",
																				 BindingFlags.NonPublic |
																				 BindingFlags.Static);
					Debug.Assert(objMethod != null);
					objMethod.Invoke(null, new object[] { dateVal, this });

				}
				if (_isAtomDateTime)
				{
					var objMethod = typeof(Atom10FeedFormatter).GetMethod("DateFromString",
																				  BindingFlags.NonPublic |
																				  BindingFlags.Instance);
					Debug.Assert(objMethod != null);
					objMethod.Invoke(new Atom10FeedFormatter(), new object[] { dateVal, this });
				}
			}
			catch (TargetInvocationException)
			{
				var dateTime = DateTimeOffset.UtcNow;
				try
				{
					dateTime = DateTime.Parse(dateVal);


				}
				catch
				{
					// ignored
				}

				if (_isRss2DateTime)
				{
					var objMethod = typeof(Rss20FeedFormatter).GetMethod("AsString",
																				  BindingFlags.NonPublic |
																				  BindingFlags.Instance);
					Debug.Assert(objMethod != null);
					return (string)(objMethod.Invoke(new Rss20FeedFormatter(), new object[] { dateTime }));
				}
				if (_isAtomDateTime)
				{
					var objMethod = typeof(Atom10FeedFormatter).GetMethod("AsString",
																				  BindingFlags.NonPublic |
																				  BindingFlags.Instance);
					Debug.Assert(objMethod != null);
					return (string)(objMethod.Invoke(new Atom10FeedFormatter(), new object[] { dateTime }));
				}
			}
			return dateVal;
		}
	}
}
