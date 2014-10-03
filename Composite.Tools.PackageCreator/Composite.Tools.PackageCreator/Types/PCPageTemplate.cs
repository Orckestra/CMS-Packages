using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Core.PageTemplates;
using Composite.Core.ResourceSystem;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("PageTemplates")]
    internal class PCPageTemplate : BasePackItem, IPack
    {
        public PCPageTemplate(string name)
            : base(name)
        {
        }

        public PCPageTemplate(EntityToken entityToken)
            : base(entityToken)
        {
        }

        public override string Id
        {
            get
            {
                if (Name == null)
                {
                    if (_entityToken is PageTemplateEntityToken)
                    {
                        var pageTemplateEntityToken = (PageTemplateEntityToken)_entityToken;
                        var template = PageTemplateFacade.GetPageTemplates().FirstOrDefault(t => t.Id == pageTemplateEntityToken.TemplateId);
                        Verify.IsNotNull(template, "Faile to find page template by ID '{0}'", pageTemplateEntityToken.TemplateId);

                        Name = template.Title;
                    }
                }
                return Name;
            }
        }

        public override ResourceHandle ItemIcon
        {
            get { return new ResourceHandle("Composite.Icons", "page-template-template"); }
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
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
            var pageTemplate = PageTemplateFacade.GetPageTemplates().FirstOrDefault(t => t.Title == Id);

            if (pageTemplate == null)
                throw new InvalidOperationException(string.Format("Template '{0}' does not exists", Id));

            if (pageTemplate.GetType().Name.Contains("MasterPagePageTemplateDescriptor"))
            {
                var codeBehindFilePath = pageTemplate.GetProperty("CodeBehindFilePath");
                var filePath = pageTemplate.GetProperty("FilePath");
                creator.AddFile("~" + PathUtil.GetWebsitePath(codeBehindFilePath));
                creator.AddFile("~" + PathUtil.GetWebsitePath(filePath));
            }
            else if (pageTemplate.GetType().Name.Contains("RazorPageTemplateDescriptor"))
            {
                var virtualPath = pageTemplate.GetProperty("VirtualPath");
                creator.AddFile(virtualPath);
            }


        }

    }
}
