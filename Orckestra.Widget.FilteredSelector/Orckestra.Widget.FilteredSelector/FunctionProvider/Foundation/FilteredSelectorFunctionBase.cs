using System;
using System.Collections.Generic;

using Composite.C1Console.Security;
using Composite.Functions;

namespace Orckestra.Widget.FilteredSelector.FunctionProvider.Foundation
{
	public abstract class FilteredSelectorFunctionBase : IFunction
	{
		private readonly EntityTokenFactory _entityTokenFactory;
		internal FilteredSelectorFunctionBase(string name, string namespaceName, string description, string helpText, Type returnType, EntityTokenFactory entityTokenFactory)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Namespace = namespaceName ?? throw new ArgumentNullException(nameof(namespaceName));
			Description = description ?? throw new ArgumentNullException(nameof(description));
			HelpText = helpText ?? throw new ArgumentNullException(nameof(helpText));
			ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
			_entityTokenFactory = entityTokenFactory ?? throw new ArgumentNullException(nameof(entityTokenFactory));
			ParameterProfiles = new List<ParameterProfile>();
		}

		public string Description { get; private set; }
		public string HelpText { get; private set; }
		public string Name { get; private set; }
		public string Namespace { get; private set; }
		public IEnumerable<ParameterProfile> ParameterProfiles { get; private set; }
		public Type ReturnType { get; private set; }

		protected void AddParameterProfile(ParameterProfile pp) => ((List<ParameterProfile>)ParameterProfiles).Add(pp);

		public EntityToken EntityToken => _entityTokenFactory.CreateEntityToken(this);

		public abstract object Execute(ParameterList parameters, FunctionContextContainer context);
	}
}
