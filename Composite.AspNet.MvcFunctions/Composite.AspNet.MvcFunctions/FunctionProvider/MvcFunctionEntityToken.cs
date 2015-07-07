using System;
using Composite.C1Console.Security;
using Composite.Functions;

namespace Composite.Plugins.Functions.FunctionProviders.MvcFunctions
{
    [SecurityAncestorProvider(typeof(StandardFunctionSecurityAncestorProvider))]
    public class MvcFunctionEntityToken : EntityToken
    {
        private readonly string _id;
        public override string Id
        {
            get { return _id; }
        }

        public override string Source
        {
            get { return String.Empty; }
        }

        public override string Type
        {
            get { return String.Empty; }
        }

        internal MvcFunctionEntityToken(MvcFunctionBase function)
        {
            _id = function.Namespace + "." + function.Name;
        }

        public MvcFunctionEntityToken(string fullName)
        {
            _id = fullName;
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedData)
        {
            string type;
            string source;
            string id;

            DoDeserialize(serializedData, out type, out source, out id);

            return new MvcFunctionEntityToken(id);
        }
    }
}
