using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.Plugins.DataProvider;
using Composite.Data.Types;
using Composite.C1Console.Security;
using Composite.Core.Types;
using DataStoreMigratorAspx;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Composite.Core.Xml;
using Composite.Core.Configuration;
using Composite.C1Console.Events;
using System.IO;
using Composite.Core.IO;
using Composite.Core.Application;
using Composite;
using Composite.C1Console.Users;
using System.Configuration;
using System.Web.Configuration;
using System.Xml.Xsl;
using System.Xml;
using System.Web.Hosting;

public partial class SqlServerDataProvider : System.Web.UI.Page
{
	private const string DynamicSqlDataProviderName = "DynamicSqlDataProvider";
	private const string DynamicSqlDataProviderConnectionStringName = "c1";
	private const string DynamicXmlDataProviderName = "DynamicXmlDataProvider";
	private const string TreeDefinitionPath = "~/App_Data/Composite/TreeDefinitions/Composite.Tools.SqlServerDataProvider.xml";
	private const string DynamicXmlDataProviderConfigPath = "~/App_Data/Composite/Configuration/DynamicXmlDataProvider.config";

	private static object _lock = new object();

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!UserValidationFacade.IsLoggedIn())
		{
			Response.Redirect("/Composite/Login.aspx?ReturnUrl=" + Request.Url.PathAndQuery);
			return;
		}

		if (!Page.IsPostBack)
		{
			if (Reflection.DefaultDynamicTypeDataProviderName == DynamicSqlDataProviderName)
			{
				lblAlreadySql.Visible = true;
				wzdSqlMigrator.Visible = false;
				return;
			}

			txtConnectionString.Text = DataFacade.GetDynamicDataProviderNames()
				.Where(d => d == DynamicSqlDataProviderName)
				.Select(d => Reflection.GetDataProviderConfiguration(d).GetProperty("ConnectionString"))
				.FirstOrDefault();

			ddlSourceDataProvider.DataSource = DataFacade.GetDynamicDataProviderNames().Where(d => d == DynamicXmlDataProviderName);
			ddlSourceDataProvider.DataBind();
		}
	}

	protected void ConnectionStringValidator_ServerValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			SqlConnection conn = new SqlConnection(txtConnectionString.Text);
			conn.Open();
			conn.Close();
		}
		catch (Exception x)
		{
			ConnectionStringValidator.Text = "Connection failed. Please make sure that the connection string is valid and, if updated, restart the server.<!-- <a href='http://docs.composite.net/Composite.Tools.DataStoreMigrator' >More information</a>.--><br /><br />Error details: " + x.Message;
			args.IsValid = false;
			return;
		}
		try
		{
			using (typeof(GlobalInitializerFacade).GetStaticProperty<IDisposable>("CoreLockScope"))
			{
				using (ApplicationOnlineHandlerFacade.TurnOffScope(false))
				{
					KeyValuePair[] trParams = new KeyValuePair[] { new KeyValuePair("ConnectionStringName", DynamicSqlDataProviderConnectionStringName), new KeyValuePair("ConnectionString", txtConnectionString.Text) };
					TransformWebConfiguration("SetConnectionString.xslt", trParams);
					TransformConfiguration("SetDynamicSqlDataProvider.xslt",
						new KeyValuePair("ConnectionStringName", DynamicSqlDataProviderConnectionStringName));
				}
			}
		}
		catch (Exception x)
		{
			ConnectionStringValidator.Text = x.Message;
			if (x.InnerException != null)
				ConnectionStringValidator.Text = x.InnerException.Message;
			args.IsValid = false;
			return;
		}
	}

	private void TransformWebConfiguration(string xsltName, params KeyValuePair[] @params)
	{
		var xsltPath = this.MapPath(xsltName);
		var xslt = XDocument.Load(xsltPath);
		foreach (var param in @params)
		{
			xslt.SetParam(param.Key, param.Value);
		}
		using (typeof(GlobalInitializerFacade).GetStaticProperty<IDisposable>("CoreLockScope"))
		{
			lock (_lock)
			{
				XslCompiledTransform transformer = new XslCompiledTransform();
				XDocument resultDocument = new XDocument();
				string webConfigFilePath = HostingEnvironment.MapPath("~/web.config");
				using (XmlReader reader = xslt.CreateReader())
				{
					transformer.Load(reader);
				}
				using (XmlWriter writer = resultDocument.CreateWriter())
				{
					transformer.Transform(webConfigFilePath, writer);
				}
				resultDocument.SaveToFile(webConfigFilePath);
			}
		}
	}

	protected void SourceValidator_ServerValidate(object source, ServerValidateEventArgs args)
	{
		if (!TestOutOfBoundStringFields())
		{
			args.IsValid = false;
			return;
		}
		try
		{
			string sourceProviderName = ddlSourceDataProvider.SelectedValue;
			string targetProviderName = DynamicSqlDataProviderName;

			var dataProviderCopier = Reflection.CreateInstance("Composite.Data.DataProviderCopier", sourceProviderName, targetProviderName);

			dataProviderCopier.CallMethod("FullCopy");

			using (typeof(GlobalInitializerFacade).GetStaticProperty<IDisposable>("CoreLockScope"))
			{
				using (ApplicationOnlineHandlerFacade.TurnOffScope(false))
				{
					TransformConfiguration("SetDefaultDynamicTypeDataProvider.xslt",
						new KeyValuePair("DynamicTypeDataProviderName", DynamicSqlDataProviderName));

					var dynamicXmlDataProviderConfiguration = DataFacade.GetDynamicDataProviderNames()
						.Where(d => d == DynamicXmlDataProviderName)
						.Select(d => Reflection.GetDataProviderConfiguration(d))
						.First();

					lock (_lock)
					{
						var xmlConfigpath = PathUtil.Resolve(DynamicXmlDataProviderConfigPath);
						File.Move(xmlConfigpath, GetBakPath(xmlConfigpath));

						var storeDirectory = PathUtil.Resolve(dynamicXmlDataProviderConfiguration.GetProperty("StoreDirectory"));
						Directory.Move(storeDirectory, GetBakPath(storeDirectory));

						try
						{
							File.Delete(PathUtil.Resolve(TreeDefinitionPath));
						}
						//Ignore
						catch { }
					}
				}
			}
			ConsoleMessageQueueFacade.Enqueue(new MessageBoxMessageQueueItem
			{
				DialogType = DialogType.Message,
				Message = lblComplete.Text,
				Title = lblComplete.Text
			}, null);
			ConsoleMessageQueueFacade.Enqueue(new RebootConsoleMessageQueueItem(), null);
		}
		catch (Exception x)
		{
			SourceValidator.Text = x.Message;
			if (x.InnerException != null)
				SourceValidator.Text = x.InnerException.Message;
			args.IsValid = false;
			return;
		}
	}

	protected void TransformConfiguration(string xsltName, params KeyValuePair[] @params)
	{
		var xsltPath = this.MapPath(xsltName);
		var xslt = XDocument.Load(xsltPath);
		foreach (var param in @params)
		{
			xslt.SetParam(param.Key, param.Value);
		}
		ConfigurationServices.TransformConfiguration(xslt, false);
	}

	protected bool TestOutOfBoundStringFields()
	{
		var sb = new StringBuilder();
		var err = new StringBuilder();

		Guid guid;
		var datatypes = DataFacade.GetAllInterfaces().Where(x => x.TryGetImmutableTypeId(out guid));

		foreach (var datatype in datatypes)
		{
			try
			{
				var fields = DynamicTypeManager.GetDataTypeDescriptor(datatype).Fields;
				foreach (var dataScopeIdentifier in DataFacade.GetSupportedDataScopes(datatype))
				{
					var cultures = DataLocalizationFacade.IsLocalized(datatype) ? DataLocalizationFacade.ActiveLocalizationCultures.ToArray() : new[] { CultureInfo.InvariantCulture };
					foreach (var cultureInfo in cultures)
					{
						using (new DataScope(dataScopeIdentifier, cultureInfo))
						{
							var recordCount = 0;
							foreach (var field in fields.Where(field => field.StoreType.IsLargeString || field.StoreType.IsString))
							{
								try
								{
									int maxLength = field.StoreType.MaximumLength;
									PropertyInfo propertyName = datatype.GetPropertiesRecursively(x => x.Name == field.Name).FirstOrDefault();
									PropertyInfo propertyId = datatype.GetProperty("Id") ?? (datatype.GetProperty("PageId") ?? datatype.GetProperty("VisabilityForeignKey"));

									var records = from x in DataFacade.GetData(datatype).ToDataEnumerable() select new { Value = (propertyName != null) ? propertyName.GetValue(x, null) : string.Empty, Id = (propertyId != null) ? propertyId.GetValue(x, null) : Guid.Empty };
									foreach (var record in records.Where(record => record.Value != null).Where(record => record.Value.ToString().Length > maxLength))
									{
										if (recordCount == 0)
											sb.Append(string.Format("<tr class='datatype'><td colspan='3'><strong>{0}</strong> ({1}), {2}, {3}</td></tr>", datatype.Name, datatype, dataScopeIdentifier.Name, cultureInfo.Name));

										sb.Append(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2} ({3} chars, {4} max)</td></tr>", record.Id, record.Value, field.Name, record.Value.ToString().Length, field.StoreType.MaximumLength));
										recordCount++;
									}
								}
								catch (Exception x)
								{
									err.Append(string.Format("<tr class='error'><td>{0} ({1})</td><td>{2}</td><td>{3}</td></tr>", datatype.Name, datatype, x.Message, field.Name));
								}
							}
						}
					}
				}
			}
			catch (Exception x)
			{
				err.Append(string.Format("<tr class='error'><td>{0} ({1})</td><td colspan='2'>{2}</td></tr>", datatype.Name, datatype, x.Message));
			}
		}

		var output = string.Empty;

		if (err.Length > 0)
			output = string.Format("<tr class='error'><td colspan='3'><h1>Exceptions</h1></td></tr>{0}", err);

		if (sb.Length > 0)
		{
			output = string.Format("{0}<tr><td colspan='3'><h1>Out-of-bound string fields</h1><p>Data records listed below contain string values that exceed the maximum length specified on the data type. You need to either truncate the string values to the maximum length or edit the data type and allow for a larger length.</p></td></tr>{1}", output, sb);
			output = string.Format("<table border='0' cellpadding='3'  cellspacing='0' width='100%' class='list'>{0}</table>", output);
			SourceValidator.Text = output;
		}

		if (err.Length > 0)
			return false;
		return true;
	}

	private string GetBakPath(string path)
	{
		var index = 0;
		string result = string.Format("{0}.bak", path);
		while (File.Exists(result) || Directory.Exists(result))
		{
			result = string.Format("{0}.{1}.bak", path, ++index);
		}
		return result;
	}
}

namespace DataStoreMigratorAspx
{

	internal static class Extensions
	{
		public static void SetParam(this XDocument xslt, string name, string value)
		{
			foreach (var param in xslt.Root.Elements(Namespaces.Xsl + "param").Where(d => d.Attribute("name").Value == name))
			{
				param.Value = value;
			}
		}
	}

	internal static class Reflection
	{
		public static DataProviderData GetDataProviderConfiguration(string providerName)
		{
			return Reflection.CallStaticMethod<DataProviderData>("Composite.Data.Plugins.DataProvider.DataProviderConfigurationServices", "GetDataProviderConfiguration", providerName);
		}

		public static string DefaultDynamicTypeDataProviderName
		{
			get
			{
				return GetType("Composite.Data.Foundation.DataProviderRegistry").GetStaticProperty("DefaultDynamicTypeDataProviderName");
			}
		}

		private static Type GetType(string typeName)
		{
			return typeof(IData).Assembly.GetType(typeName);
		}

		public static object CreateInstance(string typeName, params object[] parameters)
		{
			var type = GetType(typeName);
			return Activator.CreateInstance(type, parameters);
		}

		public static object CallStaticMethod(string typeName, string methodName, params object[] parameters)
		{
			var type = typeof(IData).Assembly.GetType(typeName);
			var methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			var methodInfo = methodInfos.Where(m => m.Name == methodName && !m.IsGenericMethod).First();
			return methodInfo.Invoke(null, parameters);
		}

		public static T CallStaticMethod<T>(string typeName, string methodName, params object[] parameters)
		{
			return (T)CallStaticMethod(typeName, methodName, parameters);
		}

		public static object CallMethod(this object o, string methodName, params object[] parameters)
		{
			var type = o.GetType();
			var methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var methodInfo = methodInfos.Where(m => m.Name == methodName && !m.IsGenericMethod).First();
			return methodInfo.Invoke(o, parameters);
		}

		public static T CallMethod<T>(this object o, string methodName, params object[] parameters)
		{
			return (T)o.CallMethod(methodName, parameters);
		}

		public static string GetStaticProperty(this Type type, string propertyName)
		{
			try
			{
				var property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				return property.GetValue(null, null).ToString();
			}
			catch { }
			return null;
		}

		public static T GetStaticProperty<T>(this Type type, string propertyName)
		{
			try
			{
				var property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				return (T)property.GetValue(null, null);
			}
			catch { }
			return default(T);
		}

		public static string GetProperty(this object o, string propertyName)
		{
			try
			{
				var property = o.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				return property.GetValue(o, null).ToString();
			}
			catch { }
			return null;
		}
	}
}