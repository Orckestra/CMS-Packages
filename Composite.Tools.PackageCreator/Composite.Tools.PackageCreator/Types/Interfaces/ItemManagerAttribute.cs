using System;

namespace Composite.Tools.PackageCreator.Types
{
    public sealed class ItemManagerAttribute : Attribute
    {
        /// <exclude />
        public ItemManagerAttribute(Type type)
        {
            Type = type;
        }

        /// <exclude />
        public Type Type { get; }
    }
}