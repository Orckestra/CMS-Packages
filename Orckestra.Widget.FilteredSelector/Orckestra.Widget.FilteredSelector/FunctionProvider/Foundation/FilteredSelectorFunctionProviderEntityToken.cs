using Composite.C1Console.Security;
using Composite.Functions;

namespace Orckestra.Widget.FilteredSelector.FunctionProvider.Foundation
{
    [SecurityAncestorProvider(typeof(StandardFunctionSecurityAncestorProvider))]
    public sealed class FilteredSelectorFunctionProviderEntityToken : EntityToken
    {
        private readonly string _id;
        private readonly string _source;

        public FilteredSelectorFunctionProviderEntityToken(string source, string id)
        {
            _source = source ?? throw new System.ArgumentNullException(nameof(source));
            _id = id ?? throw new System.ArgumentNullException(nameof(id));
        }

        public override string Type => string.Empty;

        public override string Source => _source;

        public override string Id => _id;

        public override string Serialize() => DoSerialize();

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            DoDeserialize(serializedEntityToken, out _, out string source, out string id);
            return new FilteredSelectorFunctionProviderEntityToken(source, id);
        }
    }
}
