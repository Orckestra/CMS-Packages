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
using Composite.Core.WebClient.Renderings.Page;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.GeneratedTypes;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;
using Composite.Data.Validation;
using Composite.Data.Validation.ClientValidationRules;
using Composite.Forms;
using Composite.Functions;
using Composite.Core.WebClient.Captcha;

using Microsoft.Practices.EnterpriseLibrary.Validation;
using Composite.Core.Xml;

namespace Composite.Forms.Renderer
{
	public class FormsRenderer
	{
		protected static string formsRendererPath = "Frontend/Composite/Forms/Renderer/";
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
		private static FormTreeCompiler compiler;
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

			compiler = new FormTreeCompiler();

			//bindings = formHelper.GetBindings(newData);
			bindings = new Dictionary<string, object>();
			formHelper.UpdateWithNewBindings(bindings);
			formHelper.ObjectToBindings(newData, bindings);


			using (XmlReader reader = XDocument.Parse(formHelper.GetForm()).CreateReader())
			{
				try
				{
					compiler.Compile(reader, channelIdentifier, bindings, formHelper.GetBindingsValidationRules(newData));

					#region ClientValidationRules
					clientValidationRules = new Dictionary<string,List<ClientValidationRule>>();
					foreach(var item in compiler.GetField<object>("_context").GetProperty<IEnumerable>("Rebindings"))
					{
						
						var SourceProducer  = item.GetProperty<object>("SourceProducer");
						var uiControl = SourceProducer as IWebUiControl;
						if(uiControl != null)
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
			webUiControl = (IWebUiControl)compiler.UiControl;

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
				compiler.SaveControlProperties();
			}
			catch { }
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

			bool isValid = true;
			bool useCaptcha = parameters.GetParameter<bool>("UseCaptcha");
			if (useCaptcha)
			{
				var Session = HttpContext.Current.Session;
				if (Session["FormsRendererCaptcha"] == null || !Captcha.IsValid(captchaText, Session["FormsRendererCaptcha"].ToString()))
				{
					ErrorSummary.AddError(GetFrontendString("Composite.Plugins.FormsRenderer", "Composite.Forms.Captcha.CaptchaText.error"));
					isValid = false;
				}
			}

			if (validationResults.IsValid == false)
			{
				isValid = false;

				Dictionary<string, string> _errorSummary = new Dictionary<string, string>();

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

						string _error = GetLocalized(label) + ": " + GetLocalized(help);

						if (!_errorSummary.ContainsValue(_error))
						{
							_errorSummary.Add(_errorSummary.Count().ToString(), _error);
						}
					}
					catch { }
				}

				// add errors to ErrorSummary
				foreach (var dict in _errorSummary)
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
						var formEmailHeaders = parameters.GetParameter("Email") as IEnumerable<FormEmailHeader>;

						if (formEmailHeaders != null)
						{
							//var page = HttpContext.Current.Handler as Page;
							//var attachments = new List<Attachment>();
							//if (page != null)
							//{
							//    foreach (string fileName in page.Request.Files)
							//    {
							//        HttpPostedFile file = page.Request.Files[fileName];
							//        attachments.Add(new Attachment(file.InputStream, file.FileName, file.ContentType));
							//    }
							//}

							foreach (var formEmailHeader in formEmailHeaders)
							{
								XElement inputXml = GetXElement(newData);
								XDocument mailBody = new XDocument();

								XslCompiledTransform xslTransform =  new XslCompiledTransform();
								
								xslTransform.LoadFromPath(FormsRendererLocalPath + "Xslt/MailBody.xslt");

								using (var writer = mailBody.CreateWriter())
								{
									xslTransform.Transform(inputXml.CreateReader(), writer);
								}

								MailMessage msgMail = new MailMessage();
								try
								{
									msgMail.From = new MailAddress(formEmailHeader.From);
								}
								catch (Exception e)
								{
									LoggingService.LogError(string.Format("Mail sending(From: '{0}')", formEmailHeader.From), e.Message);
									continue;
								}
								try
								{
									msgMail.To.Add(formEmailHeader.To);
								}
								catch (Exception e)
								{
									LoggingService.LogError(string.Format("Mail sending(To: '{0}')", formEmailHeader.To), e.Message);
									continue;
								}
								if (!string.IsNullOrEmpty(formEmailHeader.Cc))
								{
									try
									{
										msgMail.CC.Add(formEmailHeader.Cc);
									}
									catch (Exception e)
									{
										LoggingService.LogError(string.Format("Mail sending(Cc: '{0}')", formEmailHeader.Cc), e.Message);
									}
								}

								//foreach (var attachment in attachments)
								//{
								//    try
								//    {
								//        msgMail.Attachments.Add(attachment);
								//    }
								//    catch (Exception e)
								//    {
								//        LoggingService.LogError(string.Format("Mail sending(Attachment: '{0}')", attachment.Name), e.Message);
								//    }
								//}

								try
								{
									msgMail.Subject = formEmailHeader.Subject;
									msgMail.IsBodyHtml = true;
									msgMail.Body = mailBody.ToString();
									
									SmtpClient client = new SmtpClient();
									client.Send(msgMail);
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
			if(clientValidationRules == null)
				return false;
			if(!clientValidationRules.ContainsKey(controlId))
			{
				return false;
			}
			return clientValidationRules[controlId].Where(d => d is NotNullClientValidationRule).Any();
			
		}

		public static string GetFrontendString(string providerName, string stringId)
		{

			Verify.ArgumentNotNullOrEmpty(providerName, "providerName");
			Verify.ArgumentNotNullOrEmpty(stringId, "stringId");

			var provider = Reflection.CallStaticMethod<object>("Composite.Core.ResourceSystem.Foundation.PluginFacades.ResourceProviderPluginFacade", "GetResourceProvider", providerName);

			return provider.CallMethod<string>("GetStringValue", stringId, LocalizationScopeManager.CurrentLocalizationScope);
		}
	}

	internal static class DataExtensions
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


	}

	internal static class Reflection
	{
		public static T CallStaticMethod<T>(string typeName, string methodName, params object[] parameters)
		{
			var type = typeof(IData).Assembly.GetType(typeName);
			var methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
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
