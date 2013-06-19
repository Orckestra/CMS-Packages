using System;
using System.Text.RegularExpressions;

namespace Composite.Community.OpenID.Validators
{
    public static class RegExp
    {
        public static bool Validate(string regularExpression, object value, bool isRequired)
        {
            if (value == null)
            {
                return isRequired ? false : true;
            }
            else
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return isRequired ? false : true;
                }
                else
                {
                    return String.IsNullOrEmpty(value.ToString())
                               ? false
                               : Regex.IsMatch(value.ToString(), regularExpression, RegexOptions.IgnoreCase);
                }
            }
        }
    }
}