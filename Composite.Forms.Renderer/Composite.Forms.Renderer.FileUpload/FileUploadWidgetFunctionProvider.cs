using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Functions.Plugins.WidgetFunctionProvider;
using Composite.Functions;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Reflection;
using Composite.StandardPlugins.Functions.WidgetFunctionProviders.StandardWidgetFunctionProvider.Foundation;


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
				BindingFlags.NonPublic| BindingFlags.Instance,
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
