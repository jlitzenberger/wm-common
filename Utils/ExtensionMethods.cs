using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

namespace WM.Common
{
    public static class ExtensionMethods
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (value != null)
            {
                return value.Length <= maxLength ? value : value.Substring(0, maxLength);
            }

            return null;
        }
        public static int? ToIntOrNull(this string value)
        {
            return String.IsNullOrEmpty(value) ? (int?)null : int.Parse(value);
        }
        public static string DateToStringOrEmpty(this DateTime? value)
        {
            return value.HasValue ? string.Empty : value.ToString();
        }
        public static string EntityToStringOrEmpty(this object value)
        {
            return value == null ? string.Empty : value.ToString();
        }
        public static object TrimStringsInObj(this object obj)
        {
            var props = obj.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                // Ignore non-string properties
                    .Where(prop => prop.PropertyType == typeof(string))
                // Ignore indexers
                    .Where(prop => prop.GetIndexParameters().Length == 0)
                // Must be both readable and writable
                    .Where(prop => prop.CanWrite && prop.CanRead);

            foreach (PropertyInfo prop in props)
            {
                string value = (string)prop.GetValue(obj, null);
                if (value != null)
                {
                    value = value.Trim();
                    prop.SetValue(obj, value, null);
                }
            }
            return obj;
        }
        public static string MakeOracleInsertSafe(this string value)
        {
            string obj = value.Replace("'", "");

            return obj;
        }
        public static string EncodeUrl(this string value)
        {
            string obj = value.Replace("/", "{47}");

            obj = obj.Replace("<", "{60}");
            obj = obj.Replace(">", "{62}");
            obj = obj.Replace("*", "{42}");
            obj = obj.Replace("%", "{37}");
            obj = obj.Replace(":", "{58}");
            obj = obj.Replace("&", "{38}");
            obj = obj.Replace("#", "{35}");
            obj = obj.Replace(".", "{46}");
            obj = obj.Replace("$", "{36}");

            return obj;
        }
        public static string DecodeUrl(this string value)
        {
            string obj = value.Replace("{47}", "/");

            obj = obj.Replace("{60}", "<");
            obj = obj.Replace("{62}", ">");
            obj = obj.Replace("{42}", "*");
            obj = obj.Replace("{37}", "%");
            obj = obj.Replace("{58}", ":");
            obj = obj.Replace("{38}", "&");
            obj = obj.Replace("{35}", "#");
            obj = obj.Replace("{46}", ".");
            obj = obj.Replace("{36}", "$");

            return obj;
        }
        public static int GetNthIndex(this string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        public static string ConvertTimeToXml(this string s)
        {
            TimeSpan ts = new TimeSpan();
            TimeSpan.TryParse(s, out ts);

            return System.Xml.XmlConvert.ToString(ts);
        }
        public static string CadUtcFormat(this DateTimeOffset value)
        {
            return value != null ? value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", System.Globalization.DateTimeFormatInfo.InvariantInfo) : null;
        }
        public static string CadUtcFormatWithOffset(this DateTimeOffset value)
        {
            return value != null ? value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz", System.Globalization.DateTimeFormatInfo.InvariantInfo) : null;
        }
        public static string GetDomain(this IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(0, stop) : string.Empty;
        }
        public static string GetLogin(this IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(stop + 1, s.Length - stop - 1) : string.Empty;
        }
    }
}
