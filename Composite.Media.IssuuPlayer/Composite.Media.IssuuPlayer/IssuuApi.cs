using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Composite.Data;

namespace Composite.Media.IssuuPlayer
{
	public class IssuuApi : IDisposable
	{
		private static object _lock = new object();

		[ThreadStatic]
		private static IData _data;

		private static IData Data
		{
			get
			{
				if (_data == null)
					throw new InvalidOperationException("Api Key is not configured");
				return _data;
			}
		}

		private static string ApiKey
		{
			get
			{
				return GetProperty<string>(Data, "ApiKey");
			}
		}

		private static string ApiSecret
		{
			get
			{
				return GetProperty<string>(Data, "ApiSecret");
			}
		}

		public IssuuApi(IData data)
		{
			if (_data != null)
				throw new InvalidOperationException("Api Key already configured");
			_data = data;
		}

		public static T GetProperty<T>(IData data, string name)
		{
			var propertyInfo = data.GetType().GetProperty(name);
			if (propertyInfo == null)
			{
				throw new InvalidOperationException(string.Format("Datatype '{0}' does not have property '{1}'", data.GetType(), name));
			}
			MethodInfo getMethodInfo = propertyInfo.GetGetMethod();
			return (T)getMethodInfo.Invoke(data, null);
		}

		public void Dispose()
		{
			_data = null;
		}

		public static void SetDefault(ApiKey data)
		{
			lock (_lock)
			{
				var id = data.Id;
				var datas = DataFacade.GetData<ApiKey>().Where(d => d.Default && d.Id != id).ToArray();
				foreach (var d in datas)
				{
					d.Default = false;
				}
				DataFacade.Update(datas);
				data.Default = true;
				DataFacade.Update(data);
			}
		}

		private static IEnumerable<ApiKey> GetDefaults()
		{
			return DataFacade.GetData<ApiKey>().Where(d => d.Default);
		}

		public static string GetDefaultLabel()
		{
			lock (_lock)
			{
				return GetDefaults().Select(d => d.Title).FirstOrDefault();
			}
		}

		public static Guid GetDefaultId()
		{
			lock (_lock)
			{
				return GetDefaults().Select(d => d.Id).FirstOrDefault();
			}
		}

		public static NameValueCollection NewQuery(string action)
		{
			var data = new NameValueCollection();
			data.Add("action", action);
			data.Add("apiKey", ApiKey);
			return data;
		}

		public static void Sign(NameValueCollection data)
		{
			var aggregate = data.AllKeys.OrderBy(d => d).Aggregate(ApiSecret, (a, d) => a + d + data[d]);
			data.Add("signature", GetMd5Hash(aggregate));
		}

		// Hash an input string and return the hash as
		// a 32 character hexadecimal string.
		private static string GetMd5Hash(string input)
		{
			// Create a new instance of the MD5CryptoServiceProvider object.
			MD5 md5Hasher = MD5.Create();

			// Convert the input string to a byte array and compute the hash.
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}
	}
}
