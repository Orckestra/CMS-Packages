using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.C1Console.Security;
using Composite.Core.Types;
using DataStoreMigratorAspx;

public partial class DataStoreMigrator : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!UserValidationFacade.IsLoggedIn())
        {
            Response.Redirect("/Composite/Login.aspx?ReturnUrl=" + Request.Url.PathAndQuery);
            return;
        }

        if (!Page.IsPostBack)
        {
            var dynamicDataProviders = DataFacade.GetDynamicDataProviderNames();

            ddlSourceDataProvider.DataSource = dynamicDataProviders;
            ddlSourceDataProvider.DataBind();
            ddlTargetDataProvider.DataSource = dynamicDataProviders;
            ddlTargetDataProvider.DataBind();

            const string xmlDataProviderName = "DynamicXmlDataProvider";
            const string sqlDataProviderName = "DynamicSqlDataProvider";

            foreach (var dataProviderName in new[] { xmlDataProviderName, sqlDataProviderName })
            {
                if (!dynamicDataProviders.Contains(dataProviderName))
                {
                    throw new InvalidOperationException(string.Format("Missing data provider '{0}'", dataProviderName));
                }
            }


            ddlSourceDataProvider.Items.FindByValue(xmlDataProviderName).Selected = true;
            ddlTargetDataProvider.Items.FindByValue(sqlDataProviderName).Selected = true;

            btnTestOutOfBoundStringFields.Visible = true;
            btnTestTargetSQLConnection.Visible = true;
        }
    }

    protected void btnMigrate_Click(object sender, EventArgs e)
    {
        if (ddlSourceDataProvider.SelectedValue == ddlTargetDataProvider.SelectedValue)
        {
            lblResult.Text = "Source and Target cannot be the same!";
            return;
        }

        string sourceProviderName = ddlSourceDataProvider.SelectedValue;
        string targetProviderName = ddlTargetDataProvider.SelectedValue;

        var dataProviderCopier = Reflection.CreateInstance("Composite.Data.DataProviderCopier", sourceProviderName, targetProviderName);

        dataProviderCopier.CallMethod("FullCopy");

        lblResult.Text = "Data store migration completed";
        btnMigrate.Enabled = false;
    }

    protected void btnTestTargetSQLConnection_Click(object sender, EventArgs e)
    {
        TestSQLConnection(ddlTargetDataProvider.SelectedValue);
    }

    protected void btnTestSourceSQLConnection_Click(object sender, EventArgs e)
    {
        TestSQLConnection(ddlSourceDataProvider.SelectedValue);
    }

    private bool TestSQLConnection(string providerName)
    {
        var dataProviderSettings = Reflection.CallStaticMethod<object>("Composite.Data.Plugins.DataProvider.DataProviderConfigurationServices", "GetDataProviderConfiguration", providerName);
        var connectionString = dataProviderSettings.GetProperty("ConnectionString");

        if (string.IsNullOrEmpty(connectionString))
        {
            string connectionStringName = dataProviderSettings.GetProperty("ConnectionStringName");
            if (connectionStringName != null)
            {
                var connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionStringName];
                if (connectionStringSetting == null)
                {
                    lblResult.Text = string.Format("Failed to find connection string by name '{0}'", connectionStringName);
                    lblResult.CssClass = "error";
                    return false;
                }

                connectionString = connectionStringSetting.ConnectionString;
            }
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            lblResult.Text = "Connection string is null or empty";
            lblResult.CssClass = "error";
            return false;
        }

        try
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                conn.Close();
            }

            lblResult.Text = "Connection is OK. Continue with migration...";
            lblResult.CssClass = "info";

            return true;
        }
        catch (Exception x)
        {
            lblResult.Text = "Connection failed. Please make sure that the connection string is valid and, if updated, restart the server. <a href='http://docs.composite.net/Composite.Tools.DataStoreMigrator' >More information</a>.<br /><br />Error details: " + x.Message;
            lblResult.CssClass = "error";

            return false;
        }
    }

    protected void ddlSourceDataProvider_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnTestSourceSQLConnection.Visible = ddlSourceDataProvider.SelectedValue == "DynamicSqlDataProvider";
        btnTestOutOfBoundStringFields.Visible = ddlSourceDataProvider.SelectedValue == "DynamicXmlDataProvider"; ;
    }

    protected void ddlTargetDataProvider_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnTestTargetSQLConnection.Visible = ddlTargetDataProvider.SelectedValue == "DynamicSqlDataProvider"; ;
    }

    protected void btnTestOutOfBoundStringFields_Click(object sender, EventArgs e)
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
        }
        else
        {
            output = string.Format("{0}<tr><td colspan='3'><p>No out-of-bound string fields have been found. Please continue...</p></td></tr>{0}", output);
        }

        output = string.Format("<table border='0' cellpadding='3'  cellspacing='0' width='100%' class='list'>{0}</table>", output);
        lblResult.Text = output;
    }
}

namespace DataStoreMigratorAspx
{
    internal static class Reflection
    {
        public static object CreateInstance(string typeName, params object[] parameters)
        {
            var type = typeof(IData).Assembly.GetType(typeName);
            return Activator.CreateInstance(type, parameters);
        }

        public static object CallStaticMethod(string typeName, string methodName, params object[] parameters)
        {
            var type = typeof(IData).Assembly.GetType(typeName);
            var methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            var methodInfo = methodInfos.First(m => m.Name == methodName && !m.IsGenericMethod);
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
            var methodInfo = methodInfos.First(m => m.Name == methodName && !m.IsGenericMethod);
            return methodInfo.Invoke(o, parameters);
        }

        public static T CallMethod<T>(this object o, string methodName, params object[] parameters)
        {
            return (T)o.CallMethod(methodName, parameters);
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