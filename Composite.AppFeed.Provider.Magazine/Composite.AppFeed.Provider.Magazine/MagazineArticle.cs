using System;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Composite.AppFeed.Provider.Magazine
{
	#region Composite C1 data type attributes
	[AutoUpdateble()]
	[DataScope("public")]
	[RelevantToUserType("Developer")]
	[DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
	[ImmutableTypeId("a5150be2-0423-42e4-b54f-be02b3cbd2ac")]
	[KeyPropertyName("Id")]
	[Title("App Feed Content")]
	[LabelPropertyName("Title")]
	#endregion
	public interface MagazineArticle : IData
	{
		#region Data field attributes
		[ImmutableFieldId("ceb86948-7f92-46da-8656-1e3f5f43e7c8")]
		[FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
			"/ns/function/1.0\" />")]
		[StoreFieldType(PhysicalStoreFieldType.Guid)]
		[NotNullValidator()]
		[FieldPosition(-1)]
		#endregion
		Guid Id { get; set; }

		#region Data field attributes
		[ImmutableFieldId("8792c700-411a-4057-8f68-a75681d367a0")]
		[StoreFieldType(PhysicalStoreFieldType.Guid)]
		[NotNullValidator()]
		[GuidNotEmpty()]
		[FieldPosition(0)]
		[DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
		[ForeignKey(typeof(MagazineGroup), "Id", AllowCascadeDeletes = true, NullReferenceValue = "{00000000-0000-0000-0000-000000000000}")]
		[GroupByPriority(1)]
		#endregion
		Guid Group { get; set; }

		#region Data field attributes
		[ImmutableFieldId("7c938361-e872-4135-a2e7-f91c0f321c1d")]
		[StoreFieldType(PhysicalStoreFieldType.String, 64)]
		[NotNullValidator()]
		[FieldPosition(1)]
		[StringSizeValidator(0, 64)]
		[DefaultFieldStringValue("")]
		#endregion
		string Title { get; set; }

		#region Data field attributes
		[ImmutableFieldId("a50e9bbe-98e8-43d1-8f7a-3cff0a1113ca")]
		[StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
		[FieldPosition(2)]
		[NullStringLengthValidator(0, 256)]
		#endregion
		string SubTitle { get; set; }

		#region Data field attributes
		[ImmutableFieldId("42c1fe3a-ad1b-45dd-b222-77513d0ae6c0")]
		[StoreFieldType(PhysicalStoreFieldType.String, 2048, IsNullable = true)]
		[FieldPosition(3)]
		[NullStringLengthValidator(0, 2048)]
		[ForeignKey("Composite.Data.Types.IImageFile,Composite", AllowCascadeDeletes = true, NullableString = true)]
		#endregion
		string Image { get; set; }

		#region Data field attributes
		[ImmutableFieldId("7c548d3c-b50c-47d3-b576-a55a1734e2bf")]
		[FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Ut" +
			"ils.Date.Now\" />")]
		[StoreFieldType(PhysicalStoreFieldType.DateTime)]
		[NotNullValidator()]
		[DateTimeRangeValidator("1753-01-01T00:00:00", "9999-12-31T23:59:59")]
		[FieldPosition(4)]
		[DefaultFieldNowDateTimeValue()]
		#endregion
		DateTime Date { get; set; }

		#region Data field attributes
		[ImmutableFieldId("e1b314e8-84c4-4ab3-a81a-8f149048c6e1")]
		[StoreFieldType(PhysicalStoreFieldType.LargeString)]
		[NotNullValidator()]
		[FieldPosition(5)]
		[DefaultFieldStringValue("")]
		#endregion
		string Html { get; set; }
	}
}