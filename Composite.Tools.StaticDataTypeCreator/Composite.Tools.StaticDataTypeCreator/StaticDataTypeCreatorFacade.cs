using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Composite.Data.DynamicTypes;
using Microsoft.CSharp;

namespace Composite.Tools.StaticDataTypeCreator
{
	public class StaticDataTypeCreatorFacade
	{
		public static string CreateStaticDataType(Guid dataTypeId)
		{
			DataTypeDescriptor dataTypeDescriptor;
			DynamicTypeManager.TryGetDataTypeDescriptor(dataTypeId, out dataTypeDescriptor);

			var codeTypeDeclaration = Reflection.CallStaticMethod<CodeTypeDeclaration>("Composite.Data.GeneratedTypes.InterfaceCodeGenerator", "CreateCodeTypeDeclaration", dataTypeDescriptor);
			var csCompiler = new CSharpCodeProvider();
			var sb = new StringBuilder();

			using (var sw = new StringWriter(sb))
			{
				csCompiler.GenerateCodeFromMember(codeTypeDeclaration, sw, new CodeGeneratorOptions());
			}
			return sb.ToString();
		}
	}
}
