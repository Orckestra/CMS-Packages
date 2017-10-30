using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.ExperienceManagement.KeywordRedirect.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("6e385f57-9a05-461d-90ec-0f34fdd89049")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("${Orckestra.ExperienceManagement.KeywordRedirect, KeywordTitle}")]
    [LabelPropertyName("Keyword")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    public interface RedirectKeyword : IData, IProcessControlled, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("50fffedc-9e08-4ac7-b8d5-c60982ed6496")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("db0f55e5-8c3b-43b4-ad19-1cc127751d12")]
        [StoreFieldType(PhysicalStoreFieldType.String, 1024)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 1024)]
        [DefaultFieldStringValue("")]
        [FormRenderingProfile(Label = "${Orckestra.ExperienceManagement.KeywordRedirect,KeywordLabel}",HelpText = "${Orckestra.ExperienceManagement.KeywordRedirect,KeywordTooltip}")]
        string Keyword { get; set; }
        
        [ImmutableFieldId("9dabdcb4-463a-467c-bcd5-a9cba2654386")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [GuidNotEmpty]
        [FieldPosition(1)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes=true, NullReferenceValue="{00000000-0000-0000-0000-000000000000}")]
        [FormRenderingProfile(Label = "${Orckestra.ExperienceManagement.KeywordRedirect,LandingPageLabel}", HelpText = "${Orckestra.ExperienceManagement.KeywordRedirect,LandingPageTooltip}")]
        Guid LandingPage { get; set; }
    }
}