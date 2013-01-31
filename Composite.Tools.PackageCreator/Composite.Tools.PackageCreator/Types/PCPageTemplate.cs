using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;
using System.Xml.Linq;
using Composite.Core.ResourceSystem;
using Composite.Core.PageTemplates;
using Composite.Core.IO;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("PageTemplates")]
	internal class PCPageTemplate : SimplePackageCreatorItem, IPackageable
	{
		public PCPageTemplate(string name)
			: base(name)
		{
		}

		public PCPageTemplate(EntityToken entityToken)
			: base(entityToken)
		{
		}

		public override string Name
		{
			get
			{
				if (_name == null)
				{
					if (_entityToken is PageTemplateEntityToken)
					{
						var pageTemplateEntityToken = (PageTemplateEntityToken)_entityToken;
						var template = PageTemplateFacade.GetPageTemplates().FirstOrDefault(t => t.Id == pageTemplateEntityToken.TemplateId);
						Verify.IsNotNull(template, "Faile to find page template by ID '{0}'", pageTemplateEntityToken.TemplateId); 

						_name = template.Title;
					}
				}
				return _name;
			}
		}

		public override ResourceHandle ItemIcon
		{
			get { return new ResourceHandle("Composite.Icons", "page-template-template"); }
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
				if (dataEntityToken.Data is IXmlPageTemplate)
				{
					var data = (IXmlPageTemplate)dataEntityToken.Data;
					yield return new PCPageTemplate(data.Title);
				}
			}
			else if (entityToken is PageTemplateEntityToken)
			{
				yield return new PCPageTemplate(entityToken);
			}
		}

		public void Pack(PackageCreator creator)
		{
			var pageTemplate = PageTemplateFacade.GetPageTemplates().FirstOrDefault(t => t.Title == Name);
			
			if(pageTemplate == null)
				throw new InvalidOperationException(string.Format("Template '{0}' does not exists", Name));

			if (pageTemplate.GetType().Name == "MasterPagePageTemplateDescriptor")
			{
				var codeBehindFilePath = pageTemplate.GetProperty("CodeBehindFilePath");
				var filePath = pageTemplate.GetProperty("FilePath");
				creator.AddFile("~" + PathUtil.GetWebsitePath(codeBehindFilePath));
				creator.AddFile("~" + PathUtil.GetWebsitePath(filePath));
			}
			else if (pageTemplate.GetType().Name == "RazorPageTemplateDescriptor")
			{
				var virtualPath = pageTemplate.GetProperty("VirtualPath");
				creator.AddFile(virtualPath);
			}


		}

	}
}
