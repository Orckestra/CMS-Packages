using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;


namespace Composite.Community.Raty
{

	public class RatyCache
	{
		public RatyCache()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private const string RATY_CACHE_KEY = "Composite.Community.Raty.CacheByUniqueNumber";
		// ramdom number for security
		private const int RATY_CHECKSUM = 1735;

		private static Dictionary<Guid, List<string>> _ratyCacheByUniqueNumber;
		private static Dictionary<Guid, List<string>> RatyCacheByUniqueNumber
		{
			get
			{
				if (HttpRuntime.Cache.Get(RATY_CACHE_KEY) != null)
				{
					_ratyCacheByUniqueNumber = (Dictionary<Guid, List<string>>)HttpRuntime.Cache.Get(RATY_CACHE_KEY);
				}
				else
				{
					_ratyCacheByUniqueNumber = new Dictionary<Guid, List<string>>();
					HttpRuntime.Cache.Add(RATY_CACHE_KEY,
						_ratyCacheByUniqueNumber,
						null,
						DateTime.Now.AddDays(30),
						Cache.NoSlidingExpiration,
						CacheItemPriority.Normal,
						null);
				}
				return _ratyCacheByUniqueNumber;
			}
		}

		public static void SaveVotedInMemory(Guid ratyId, string uniqueNumber)
		{
			List<string> uniqueNumbers = new List<string>();
			if (RatyCacheByUniqueNumber.TryGetValue(ratyId, out uniqueNumbers))
			{
				if (!uniqueNumbers.Contains(uniqueNumber))
				{
					uniqueNumbers.Add(uniqueNumber);
				}
			}
			else
			{
				uniqueNumbers = new List<string>();
				uniqueNumbers.Add(uniqueNumber);
				RatyCacheByUniqueNumber[ratyId] = uniqueNumbers;
			}
		}

		public static bool IsAlreadyVoted(Guid ratyId)
		{
			List<string> uniqueNumbers = new List<string>();
			string currentUniqueNumber = GetUniqueNumber().ToString();
			RatyCacheByUniqueNumber.TryGetValue(ratyId, out uniqueNumbers);
			return uniqueNumbers != null ? uniqueNumbers.Contains(currentUniqueNumber) : false;
		}

		public static bool CheckUniqueNumber(int uniqueNumber)
		{
			return uniqueNumber == GetUniqueNumber();
		}

		public static XElement GetRatyInformation(Guid ratyId)
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			return new XElement("RatyInfo", new XAttribute("UniqueNumber", GetUniqueNumber().ToString()), new XAttribute("IsAlreadyVoted", IsAlreadyVoted(ratyId).ToString().ToLower()));
		}

		private static int GetUniqueNumber()
		{
			string uniqueTemplate = "{0} {1}";
			var currectRequest = HttpContext.Current.Request;
			string uniqueString = string.Format(uniqueTemplate, currectRequest.UserHostAddress, currectRequest.UserAgent);
			int uniqueNumber = uniqueString.GetHashCode() + RATY_CHECKSUM;

			return uniqueNumber;

		}
	}
}