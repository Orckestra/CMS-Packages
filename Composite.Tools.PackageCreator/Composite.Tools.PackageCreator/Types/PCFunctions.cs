using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Functions;
using Composite.AspNet.Security;
using Composite.Core.IO;


	namespace Composite.Tools.PackageCreator.Types
	{
		[PCCategory("Functions", AliasNames = new[] { "CSharpFunctions", "InlineFunctions", "RazorFunctions", "VisualFunctions", "XsltFunctions" })]
		partial class PCFunctions : SimplePackageCreatorItem, IPackageable
		{

			public PCFunctions(string name)
				: base(name)
			{
			}

			public override ResourceHandle ItemIcon
			{
				get { return new ResourceHandle("Composite.Icons", "base-function-function"); }
			}

			public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
			{
				return CreateCSharp(entityToken)
					.Concat(CreateInline(entityToken))
					.Concat(CreateRazor(entityToken))
					.Concat(CreateUserControl(entityToken))
					.Concat(CreateVisual(entityToken))
					.Concat(CreateXslt(entityToken));

			}

			public void Pack(PackageCreator pc)
			{
				
				IFunction function = null;
				if (FunctionFacade.TryGetFunction(out function, this.Name))
				{

					var innerFunction = function.GetProperty<IFunction>("InnerFunction");
					var innerFunctionTypeName = innerFunction.GetType().Name;
					
					switch(innerFunctionTypeName)
					{
						case "MethodBasedFunction":
							PackCSharpFunction(pc);
							break;
						case "InlineFunction":
							PackInlineFunction(pc);
							break;
						case "RazorBasedFunction":
							PackRazorFunction(pc, innerFunction);
							break;
						case "UserControlBasedFunction":
							PackUserControlFunction(pc, innerFunction);
							break;
						case "VisualFunction`1":
							PackVisualFunction(pc);
							break;
						case "XsltXmlFunction":
							PackXsltFunction(pc);
							break;
					}
					


					
				}
				else
				{
					throw new InvalidOperationException(string.Format("Function '{0}' does not exists", this._name));
				}
			}
		}

	}

