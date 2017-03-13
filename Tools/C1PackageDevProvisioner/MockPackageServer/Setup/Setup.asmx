<%@ WebService Language="C#" Class="Setup" %>

using System;
using System.Web.Services;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Linq;


[WebService(Namespace = "http://www.composite.net/ns/management")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Setup : System.Web.Services.WebService
{

    [WebMethod]
    public bool Ping()
    {
        return true;
    }



    [WebMethod]
    public XmlDocument GetSetupDescription(string version, string installationId)
    {
	    var ver = Version.Parse(version);
        Guid instId = Guid.Parse(installationId);

        string filepath = Server.MapPath("../App_Data/Setup/SetupDescription.xml");

	    var xml = File.ReadAllText(filepath)
                      .Replace("$(version)", ver.ToString())
                      .Replace("$(installationId)", instId.ToString());

        var doc = new XmlDocument();
        doc.LoadXml(xml);

        return doc;
    }




    [WebMethod]
    public XmlDocument GetGetLicense(string version, string installationId)
    {
        string filepath = Server.MapPath("../App_Data/Setup/License.xml");

        var doc = new XmlDocument();
        doc.Load(filepath);

        return doc;
    }



    [WebMethod]
    public XmlDocument GetLanguages(string version, string installationId)
    {
        string filepath = Server.MapPath("../App_Data/Setup/Languages.xml");

        var doc = new XmlDocument();
        doc.Load(filepath);

        return doc;
    }



    [WebMethod]
    public XmlDocument GetLanguagePackages(string version, string installationId)
    {
        string filepath = Server.MapPath("../App_Data/Setup/LanguagePackages.xml");

        var doc = new XmlDocument();
        doc.Load(filepath);

        return doc;
    }



    /// <summary>
    ///
    /// </summary>
    /// <param name="version"></param>
    /// <param name="installationId"></param>
    /// <param name="setupDescriptionXml"></param>
    /// <param name="exception">If any exception occures, this will contain it. Otherwise this will be an empty string</param>
    /// <returns></returns>
    [WebMethod]
    public bool RegisterSetup(string version, string installationId, string setupDescriptionXml, string exception)
    {
        try
        {
            Guid id = new Guid(installationId);
            if (version.Length > 20) throw new ArgumentException("version");

            var ip = this.Context.Request.UserHostAddress;

            Task.Factory.StartNew(() => RegisterSetupBlocking(id, version, ip, setupDescriptionXml, exception));

            return true;
        }
        catch (Exception ex)
        {
            string errorFilePath = Server.MapPath("../App_Data/Setup/RegisterSetupExceptions.txt");
            if (!File.Exists(errorFilePath))
            {
                File.WriteAllText(errorFilePath, "Exceptions thrown from Setup.asmx/RegisterSetup:\n\n");
            }

            File.AppendAllText(errorFilePath, string.Format("Exception thrown {0}:\n{1}\n\n", DateTime.Now, ex.ToString()));
        }

        return false;
    }

    private void RegisterSetupBlocking(Guid installationId, string version, string ip, string setupDescriptionXml, string exception)
    {
        try
        {
            //			var packages = new CompositeC1SetupRegistrationsFacade();
            //			packages.SendData(installationId, version, ip, setupDescriptionXml, exception);
        }
        catch (Exception ex)
        {
            string errorFilePath = Server.MapPath("../App_Data/Setup/RegisterSetupBlockingExceptions.txt");
            if (!File.Exists(errorFilePath))
            {
                File.WriteAllText(errorFilePath, "Exceptions thrown from Setup.asmx/RegisterSetupBlocking:\n\n");
            }

            File.AppendAllText(errorFilePath, string.Format("Exception thrown {0}:\n{1}\n\n", DateTime.Now, ex.ToString()));
        }
    }

}