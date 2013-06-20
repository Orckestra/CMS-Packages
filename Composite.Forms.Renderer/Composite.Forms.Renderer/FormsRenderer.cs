using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Composite.C1Console.Forms;
using Composite.C1Console.Forms.WebChannel;
using Composite.Core.IO;
using Composite.Core.Logging;
using Composite.Core.ResourceSystem;
using Composite.Core.Types;
using Composite.Core.WebClient.Captcha;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.GeneratedTypes;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;
using Composite.Data.Validation;
using Composite.Data.Validation.ClientValidationRules;
using Composite.Functions;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Composite.C1Console.Elements;
using Composite.Plugins.Elements.ElementProviders.MediaFileProviderElementProvider;
using System.Linq.Expressions;

namespace Composite.Forms.Renderer
{
    public class FormsRenderer
    {
        protected static string formsRendererPath = "Frontend/Composite/Forms/Renderer/";

        private static readonly Expression IgnoreCaseConstantExpression = Expression.Constant(StringComparison.OrdinalIgnoreCase, typeof(StringComparison));
        private static readonly MethodInfo EndsWithMethodInfo = typeof(string).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) });


        public static string FormsRendererWebPath
        {
            get
            {
                return GetApplicationPath() + formsRendererPath;
            }
        }

        public static string FormsRendererLocalPath
        {
            get
            {
                return Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), formsRendererPath);
            }
        }

        protected static string GetApplicationPath()
        {
            string path = HttpContext.Current.Request.ApplicationPath;
            return path.Length > 1 ? path + "/" : path;
        }

        [ThreadStatic]
        private static FormTreeCompiler _compiler;
        [ThreadStatic]
        private static DataTypeDescriptorFormsHelper formHelper;
        [ThreadStatic]
        private static IData newData;
        [ThreadStatic]
        private static Dictionary<string, object> bindings;
        [ThreadStatic]
        private static IWebUiControl webUiControl;

        [ThreadStatic]
        private static Dictionary<string, List<ClientValidationRule>> clientValidationRules;

        public static void InsertForm(Control control, ParameterList parameters)
        {
            Page currentPageHandler = HttpContext.Current.Handler as Page;


            if (currentPageHandler == null) throw new InvalidOperationException("The Current HttpContext Handler must be a System.Web.Ui.Page");

            Type dataType = null;
            DataTypeDescriptor dataTypeDescriptor = null;

            string dataTypeName = parameters.GetParameter<string>("DataType");

            dataType = TypeManager.GetType(dataTypeName);
            dataTypeDescriptor = DynamicTypeManager.GetDataTypeDescriptor(dataType);

            IFormChannelIdentifier channelIdentifier = FormsRendererChannel.Identifier;

            formHelper = new DataTypeDescriptorFormsHelper(dataTypeDescriptor);

            newData = DataFacade.BuildNew(dataType);
            GeneratedTypesHelper.SetNewIdFieldValue(newData);

            //Hide not editable fields, fox example - PageId
            GeneratedTypesHelper generatedTypesHelper = new GeneratedTypesHelper(dataTypeDescriptor);
            formHelper.AddReadOnlyFields(generatedTypesHelper.NotEditableDataFieldDescriptorNames);


            //If is Page Datatype
            if (PageFolderFacade.GetAllFolderTypes().Contains(dataType))
            {
                IPage currentPage = PageRenderer.CurrentPage;

                if (currentPage.GetDefinedFolderTypes().Contains(dataType) == false)
                {
                    currentPage.AddFolderDefinition(dataType.GetImmutableTypeId());
                }
                PageFolderFacade.AssignFolderDataSpecificValues(newData, currentPage);

            }

            _compiler = new FormTreeCompiler();

            //bindings = formHelper.GetBindings(newData);
            bindings = new Dictionary<string, object>();
            formHelper.UpdateWithNewBindings(bindings);
            formHelper.ObjectToBindings(newData, bindings);


            using (XmlReader reader = XDocument.Parse(formHelper.GetForm()).CreateReader())
            {
                try
                {
                    _compiler.Compile(reader, channelIdentifier, bindings, formHelper.GetBindingsValidationRules(newData));

                    #region ClientValidationRules
                    clientValidationRules = new Dictionary<string, List<ClientValidationRule>>();
                    foreach (var item in _compiler.GetField<object>("_context").GetProperty<IEnumerable>("Rebindings"))
                    {

                        var SourceProducer = item.GetProperty<object>("SourceProducer");
                        var uiControl = SourceProducer as IWebUiControl;
                        if (uiControl != null)
                        {
                            clientValidationRules[uiControl.UiControlID] = uiControl.ClientValidationRules;
                        }

                    }
                    #endregion
                }
                catch (ConfigurationErrorsException e)
                {
                    if (e.Message.Contains("Failed to load the configuration for IUiControlFactory"))
                    {
                        throw new ConfigurationErrorsException("Composite.Forms.Renderer does not support widget. " + e.Message);
                    }
                    else
                    {
                        throw new ConfigurationErrorsException(e.Message);
                    }

                }
            }
            webUiControl = (IWebUiControl)_compiler.UiControl;

            Control form = webUiControl.BuildWebControl();
            control.Controls.Add(form);

            /*if (currentPageHandler.IsPostBack)
                try
                {
                    compiler.SaveControlProperties();
                }
                catch { }*/

            if (!currentPageHandler.IsPostBack)
            {
                webUiControl.InitializeViewState();
            }



            return;
        }

        public static bool SubmitForm(ParameterList parameters, string captchaText)
        {
            try
            {
                _compiler.SaveControlProperties();
            }
            catch (Exception)
            { }
            webUiControl.InitializeViewState();

            Dictionary<string, string> errorMessages = formHelper.BindingsToObject(bindings, newData);

            DataTypeDescriptor dataTypeDescriptor = DynamicTypeManager.GetDataTypeDescriptor(newData.DataSourceId.InterfaceType);
            foreach (var property in newData.DataSourceId.InterfaceType.GetProperties())
            {
                if (property.PropertyType == typeof(string) && (string)property.GetValue(newData, null) == string.Empty)
                {
                    property.SetValue(newData, null, null);
                }
            }


            ValidationResults validationResults = ValidationFacade.Validate(newData.DataSourceId.InterfaceType, newData);

            var isValid = true;
            var useCaptcha = parameters.GetParameter<bool>("UseCaptcha");
            if (useCaptcha)
            {
                var Session = HttpContext.Current.Session;
                if (Session["FormsRendererCaptcha"] == null || !Captcha.IsValid(captchaText, Session["FormsRendererCaptcha"].ToString()))
                {
                    ErrorSummary.AddError(StringResourceSystemFacade.GetString("Composite.Forms.Renderer", "Composite.Forms.Captcha.CaptchaText.error"));
                    isValid = false;
                }
            }

            if (validationResults.IsValid == false)
            {
                isValid = false;

                Dictionary<string, string> errorSummary = new Dictionary<string, string>();

                foreach (ValidationResult result in validationResults)
                {
                    var label = result.Key;
                    var help = result.Message;

                    try
                    {
                        label = dataTypeDescriptor.Fields[result.Key].FormRenderingProfile.Label;

                        help = dataTypeDescriptor.Fields[result.Key].FormRenderingProfile.HelpText;

                        //if no HelpText specified - use standard C1 error
                        if (help == string.Empty)
                        {
                            help = result.Message;
                        }

                        string error = GetLocalized(label) + ": " + GetLocalized(help);

                        if (!errorSummary.ContainsValue(error))
                        {
                            errorSummary.Add(errorSummary.Count().ToString(), error);
                        }
                    }
                    catch { }
                }

                // add errors to ErrorSummary
                foreach (var dict in errorSummary)
                {
                    ErrorSummary.AddError(dict.Value);
                }
            }
            //TODO: Looks like rudimentary code related to old C1 errros with binding?
            if (errorMessages != null)
            {
                isValid = false;
                foreach (var kvp in errorMessages)
                {
                    var label = kvp.Key;
                    try
                    {
                        label = dataTypeDescriptor.Fields[kvp.Key].FormRenderingProfile.Label;
                    }
                    catch { }
                    ErrorSummary.AddError(GetLocalized(label) + ": " + GetLocalized(kvp.Value));
                }
            }

            if (isValid)
            {
                using (new DataScope(DataScopeIdentifier.Administrated))
                {
                    IPublishControlled publishControlled = newData as IPublishControlled;
                    if (publishControlled != null)
                    {
                        publishControlled.PublicationStatus = GenericPublishProcessController.Draft;
                    }
                    DataFacade.AddNew(newData);

                    using (var datascope = new FormsRendererDataScope(newData))
                    {
                        var formEmailHeaders = parameters.GetParameter("Email") as IEnumerable<FormEmail>;

                        if (formEmailHeaders != null)
                        {
                            foreach (var formEmail in formEmailHeaders)
                            {
                                ParameterFacade.ResolveProperties(formEmail);
                                var inputXml = GetXElement(newData);
                                var body = new XhtmlDocument();

                                body.AppendDocument(formEmail.Body);

                                if (formEmail.AppendFormData)
                                {
                                    var formData = new XDocument();
                                    var xslTransform = new XslCompiledTransform();

                                    xslTransform.LoadFromPath(FormsRendererLocalPath + "Xslt/MailBody.xslt");

                                    using (var writer = formData.CreateWriter())
                                    {
                                        xslTransform.Transform(inputXml.CreateReader(), writer);
                                    }

                                    body.AppendDocument(formData);
                                }

                                Reflection.CallStaticMethod<object>("Composite.Core.WebClient.Renderings.Page.PageRenderer", "NormalizeXhtmlDocument", body);

                                var mailMessage = new MailMessage();
                                try
                                {
                                    mailMessage.From = new MailAddress(formEmail.From);
                                }
                                catch (Exception e)
                                {
                                    LoggingService.LogError(string.Format("Mail sending(From: '{0}')", formEmail.From), e.Message);
                                    continue;
                                }
                                try
                                {
                                    mailMessage.To.Add(formEmail.To);
                                }
                                catch (Exception e)
                                {
                                    LoggingService.LogError(string.Format("Mail sending(To: '{0}')", formEmail.To), e.Message);
                                    continue;
                                }
                                if (!string.IsNullOrEmpty(formEmail.Cc))
                                {
                                    try
                                    {
                                        mailMessage.CC.Add(formEmail.Cc);
                                    }
                                    catch (Exception e)
                                    {
                                        LoggingService.LogError(string.Format("Mail sending(Cc: '{0}')", formEmail.Cc), e.Message);
                                    }
                                }

                                try
                                {
                                    mailMessage.Subject = formEmail.Subject;
                                    mailMessage.IsBodyHtml = true;
                                    mailMessage.Body = body.ToString();

                                    new SmtpClient().Send(mailMessage);
                                }
                                catch (Exception e)
                                {
                                    throw new InvalidOperationException("Unable to send mail. Please ensure that web.config has correct /configuration/system.net/mailSettings: " + e.Message);
                                }
                            }
                        }
                    }

                }
            }
            return isValid;
        }

        private static string GetLocalized(string text)
        {
            return text.Contains("${") ? StringResourceSystemFacade.ParseString(text) : text;
        }

        public static XElement GetXElement(IData data)
        {
            var elementName = data.DataSourceId.InterfaceType.Name;
            XElement xml = new XElement(elementName);

            var dataTypeDescriptor = DynamicTypeManager.GetDataTypeDescriptor(data.DataSourceId.InterfaceType.GetImmutableTypeId());

            GeneratedTypesHelper generatedTypesHelper = new GeneratedTypesHelper(dataTypeDescriptor);
            //generatedTypesHelper.NotEditableDataFieldDescriptorNames

            foreach (DataFieldDescriptor fieldDescriptor in dataTypeDescriptor.Fields.Where(dfd => dfd.Inherited == false))
            {
                var propertyInfo = data.DataSourceId.InterfaceType.GetProperty(fieldDescriptor.Name);

                if (!generatedTypesHelper.NotEditableDataFieldDescriptorNames.Contains(fieldDescriptor.Name))
                {
                    string label = fieldDescriptor.FormRenderingProfile.Label;
                    object value = propertyInfo.GetValue(data, null);

                    List<ForeignKeyAttribute> foreignKeyAttributes = propertyInfo.GetCustomAttributesRecursively<ForeignKeyAttribute>().ToList();
                    if (foreignKeyAttributes.Count > 0)
                    {
                        IData foreignData = data.GetReferenced(propertyInfo.Name);

                        value = DataAttributeFacade.GetLabel(foreignData);
                    }
                    if (value == null)
                    {
                        value = string.Empty;
                    }
                    xml.Add(
                        new XElement("Property",
                            new XAttribute("Label", GetLocalized(label)),
                            new XAttribute("Value", value)
                            )
                        );
                }
            }
            return xml;
        }

        public static bool IsRequiredControl(string controlId)
        {
            if (clientValidationRules == null)
                return false;
            if (!clientValidationRules.ContainsKey(controlId))
            {
                return false;
            }
            return clientValidationRules[controlId].Where(d => d is NotNullClientValidationRule).Any();

        }

        public static Func<IMediaFile, bool> GetPredicate(SearchToken searchToken)
        {
            var predicates = new List<Func<IMediaFile, bool>>();
            if (searchToken is MediaFileSearchToken)
            {
                MediaFileSearchToken mediaFileSearchToken = (MediaFileSearchToken)searchToken;
                if (mediaFileSearchToken.MimeTypes != null && mediaFileSearchToken.MimeTypes.Length > 0)
                {
                    List<string> mimeTypes = new List<string>(mediaFileSearchToken.MimeTypes);
                    predicates.Add(x => mimeTypes.Contains(x.MimeType));
                }
                if (!string.IsNullOrEmpty(mediaFileSearchToken.Folder))
                {
                    if (mediaFileSearchToken.HideSubfolders)
                    {
                        predicates.Add(d => d.FolderPath == mediaFileSearchToken.Folder);
                    }
                    else
                    {
                        predicates.Add(d => d.FolderPath.StartsWith(mediaFileSearchToken.Folder));
                    }
                }

                if (mediaFileSearchToken.Extensions != null && mediaFileSearchToken.Extensions.Length > 0)
                {
                    System.Linq.Expressions.ParameterExpression fileParameter = Expression.Parameter(typeof(IMediaFile), "file");

                    Expression body = null;

                    foreach (string extension in mediaFileSearchToken.Extensions)
                    {
                        string suffix = extension.StartsWith(".") ? extension : "." + extension;

                        // "file.FileName"
                        Expression fileName = Expression.Property(fileParameter, typeof(IFile), "FileName");

                        // Building "file.FileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)"
                        MethodCallExpression predicate = Expression.Call(fileName,
                                                                         EndsWithMethodInfo,
                                                                         Expression.Constant(suffix),
                                                                         IgnoreCaseConstantExpression);

                        if (body == null)
                        {
                            // file => file.FileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
                            body = predicate;
                        }
                        else
                        {
                            // body = (.....) || file.FileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase;
                            body = Expression.OrElse(body, predicate);
                        }
                    }

                    predicates.Add(Expression.Lambda<Func<IMediaFile, bool>>(body, fileParameter).Compile());
                }

            }

            if (predicates.Count == 0)
            {
                return (x => true);
            }

            Func<Func<IMediaFile, bool>, Func<IMediaFile, bool>, Func<IMediaFile, bool>>
                and = (f1, f2) => (t => f1(t) && f2(t));

            Func<IMediaFile, bool> current = (x => true);
            foreach (Func<IMediaFile, bool> predicate in predicates)
            {
                current = and(predicate, current);
            }
            return current;
        }

    }

    internal static class FormsRendererExtensions
    {
        public static T GetField<T>(this object data, string fieldName)
        {
            var fieldInfo = data.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                return default(T);
            }
            return (T)fieldInfo.GetValue(data);
        }


        public static T GetProperty<T>(this object data, string propertyName)
        {
            var propertyInfo = data.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo == null)
            {
                return default(T);
            }
            MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
            return (T)getMethodInfo.Invoke(data, null);
        }

        public static void AppendDocument(this XhtmlDocument document1, XDocument document)
        {
            document1.Body.Add(document.Root);
        }
    }

    internal static class Reflection
    {
        public static T CallStaticMethod<T>(string typeName, string methodName, params object[] parameters)
        {
            var type = typeof(IData).Assembly.GetType(typeName);
            var methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            var methodInfo = methodInfos.Where(m => m.Name == methodName && !m.IsGenericMethod).First();
            return (T)methodInfo.Invoke(null, parameters);
        }

        public static T CallMethod<T>(this object o, string methodName, params object[] parameters)
        {
            var type = o.GetType();
            var methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var methodInfo = methodInfos.Where(m => m.Name == methodName && !m.IsGenericMethod).First();
            return (T)methodInfo.Invoke(o, parameters);
        }

    }
}
