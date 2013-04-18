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
	[ImmutableTypeId("2f1e0997-6791-4ccb-9789-437319c8d789")]
	[KeyPropertyName("Id")]
	[Title("App Feed Group")]
	[LabelPropertyName("Title")]
	#endregion
	public interface MagazineGroup : IData
	{
		#region Data field attributes
		[ImmutableFieldId("d845feba-18df-4733-b7d7-4a7e4a844751")]
		[FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
			"/ns/function/1.0\" />")]
		[StoreFieldType(PhysicalStoreFieldType.Guid)]
		[NotNullValidator()]
		[FieldPosition(-1)]
		#endregion
		Guid Id { get; set; }

		#region Data field attributes
		[ImmutableFieldId("87dcb71f-9421-4052-a73d-8efd08af236f")]
		[StoreFieldType(PhysicalStoreFieldType.String, 16)]
		[FieldPosition(0)]
		[StringSizeValidator(0, 16)]
		[DefaultFieldStringValue("")]
		#endregion
		string Name { get; set; }

		#region Data field attributes
		[ImmutableFieldId("3035c21d-d92d-41be-b3bc-339491e3ebc8")]
		[StoreFieldType(PhysicalStoreFieldType.String, 64)]
		[NotNullValidator()]
		[FieldPosition(1)]
		[StringSizeValidator(0, 64)]
		[DefaultFieldStringValue("")]
		#endregion
		string Title { get; set; }

		#region Data field attributes
		[ImmutableFieldId("a908c367-40a2-440a-9398-a531faea56d1")]
		[StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
		[FieldPosition(2)]
		[NullStringLengthValidator(0, 256)]
		#endregion
		string SubTitle { get; set; }

		#region Data field attributes
		[ImmutableFieldId("0d38e5f6-a4fc-4a63-b953-330562b89716")]
		[FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
			"nstant.Integer\"><f:param name=\"Constant\" value=\"50\" /></f:function>")]
		[StoreFieldType(PhysicalStoreFieldType.Integer)]
		[NotNullValidator()]
		[FieldPosition(3)]
		[IntegerRangeValidator(-2147483648, 2147483647)]
		[DefaultFieldIntValue(0)]
		#endregion
		int Priority { get; set; }

		#region Data field attributes
		[ImmutableFieldId("28c59ed3-821b-415c-81a2-4dd72e912edb")]
		[StoreFieldType(PhysicalStoreFieldType.String, 2048, IsNullable = true)]
		[FieldPosition(4)]
		[NullStringLengthValidator(0, 2048)]
		[ForeignKey("Composite.Data.Types.IImageFile,Composite", AllowCascadeDeletes = true, NullableString = true)]
		#endregion
		string Image { get; set; }

		#region Data field attributes
		[ImmutableFieldId("9aa77fa1-6c9d-4802-8538-42e2394e12c6")]
		[StoreFieldType(PhysicalStoreFieldType.LargeString)]
		[NotNullValidator()]
		[FieldPosition(5)]
		[DefaultFieldStringValue("")]
		#endregion
		string Html { get; set; }
	}
}