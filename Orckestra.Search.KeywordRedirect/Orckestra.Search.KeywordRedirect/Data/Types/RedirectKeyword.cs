using System;
using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Orckestra.Search.KeywordRedirect.Data.Types
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("6e385f57-9a05-461d-90ec-0f34fdd89049")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("${Orckestra.Search.KeywordRedirect, KeywordTitle}")]
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
        [FormRenderingProfile(Label = "${Orckestra.Search.KeywordRedirect,KeywordLabel}",HelpText = "${Orckestra.Search.KeywordRedirect,KeywordTooltip}")]
        string Keyword { get; set; }

        [ImmutableFieldId("a9b04b80-8cb2-483d-8520-e54e71b70763")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable=true)]
        [FieldPosition(1)]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes=true)]
        [GroupByPriority(1)]
        [FormRenderingProfile(Label = "${Orckestra.Search.KeywordRedirect,HomePageLabel}", HelpText = "${Orckestra.Search.KeywordRedirect,HomePageTooltip}")]
        Nullable<Guid> HomePage { get; set; }

        [ImmutableFieldId("9dabdcb4-463a-467c-bcd5-a9cba2654386")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [GuidNotEmpty]
        [FieldPosition(2)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes=true, NullReferenceValue="{00000000-0000-0000-0000-000000000000}")]
        [FormRenderingProfile(Label = "${Orckestra.Search.KeywordRedirect,LandingPageLabel}", HelpText = "${Orckestra.Search.KeywordRedirect,LandingPageTooltip}")]
        Guid LandingPage { get; set; }
    }
}