using System.Linq;

namespace Composite.Tools.StaticDataTypeCreator
{
	public static class Extensions
	{
		public static bool In<T>(this T value, params T[] items)
		{
			return items.Contains(value);
		}

		public static string GetStringInBetween(this string strSource, string strBegin, string strEnd)
		{
			return GetStringInBetween(strSource, strBegin, strEnd, false, false)[0];
		}

		public static string[] GetStringInBetween(this string strSource, string strBegin, string strEnd, bool includeBegin, bool includeEnd)
		{
			string[] result = { "", "" };

			int iIndexOfBegin = strSource.IndexOf(strBegin);

			if (iIndexOfBegin != -1)
			{
				// include the Begin string if desired

				if (includeBegin)

					iIndexOfBegin -= strBegin.Length;

				strSource = strSource.Substring(iIndexOfBegin

					+ strBegin.Length);

				int iEnd = strSource.IndexOf(strEnd);

				if (iEnd != -1)
				{

					// include the End string if desired

					if (includeEnd)

						iEnd += strEnd.Length;

					result[0] = strSource.Substring(0, iEnd);

					// advance beyond this segment

					if (iEnd + strEnd.Length < strSource.Length)

						result[1] = strSource.Substring(iEnd

							+ strEnd.Length);
				}
			}

			else

				// stay where we are

				result[1] = strSource;

			return result;
		}
	}
}
