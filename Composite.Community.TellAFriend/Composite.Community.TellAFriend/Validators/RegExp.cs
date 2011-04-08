using System;
using System.Text.RegularExpressions;

namespace Composite.Community.TellAFriend.Validators
{
	public static class RegExp
	{
		static public bool Validate(string regularExpression, object value, bool isRequired)
		{
			if (value == null)
			{
				return isRequired ? false : true;
			}
			{
				return String.IsNullOrEmpty(value.ToString()) ? false : Regex.IsMatch(value.ToString(), regularExpression, RegexOptions.IgnoreCase);
			}
		}
	}
}
