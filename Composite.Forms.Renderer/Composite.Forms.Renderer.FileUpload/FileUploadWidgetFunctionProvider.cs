using System;
using System.Collections.Generic;
using System.Reflection;
using Composite.Functions;
using Composite.Functions.Plugins.WidgetFunctionProvider;
using Composite.Plugins.Functions.WidgetFunctionProviders.StandardWidgetFunctionProvider.Foundation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder;


namespace Composite.Forms.Renderer.FileUpload
{
	[ConfigurationElementType(typeof(FileUploadWidgetFunctionProviderData))]
	public class FileUploadWidgetFunctionProvider : IWidgetFunctionProvider
	{
		private EntityTokenFactory _entityTokenFactory;
		private WidgetFunctionNotifier _widgetFunctionNotifier;
		private List<IWidgetFunction> _widgetStaticTypeFunctions = null;

		public FileUploadWidgetFunctionProvider(string providerName)
		{
			_entityTokenFactory = (EntityTokenFactory)Activator.CreateInstance(
				typeof(EntityTokenFactory),
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
				null,
				new object[]{ providerName},
				null);
		}

		public IEnumerable<Functions.IWidgetFunction> Functions
		{
			get
			{
				if (_widgetStaticTypeFunctions == null)
				{
					InitializeStaticTypeFunctions();
				}

				foreach (IWidgetFunction widgetFunction in _widgetStaticTypeFunctions)
				{
					yield return widgetFunction;
				}
			}
		}

		public WidgetFunctionNotifier WidgetFunctionNotifier
		{
			set { _widgetFunctionNotifier = value; }
		}

		private void InitializeStaticTypeFunctions()
		{
			_widgetStaticTypeFunctions = new List<IWidgetFunction>();
			_widgetStaticTypeFunctions.Add(new FrontendFileUploadWidgetFunction(_entityTokenFactory));
		}
	}

	[Assembler(typeof(ExtranetWidgetFunctionProviderAssembler))]
	public class FileUploadWidgetFunctionProviderData : WidgetFunctionProviderData
	{
	}

	public sealed class ExtranetWidgetFunctionProviderAssembler : IAssembler<IWidgetFunctionProvider, WidgetFunctionProviderData>
	{
		public IWidgetFunctionProvider Assemble(IBuilderContext context, WidgetFunctionProviderData objectConfiguration, IConfigurationSource configurationSource, ConfigurationReflectionCache reflectionCache)
		{
			return new FileUploadWidgetFunctionProvider(objectConfiguration.Name);
		}
	}
}
