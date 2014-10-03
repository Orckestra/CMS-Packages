using System;

namespace Composite.Tools.PackageCreator.Types
{
    /// <summary>
    /// All classes that inherit <see cref="IPackItem"/> and has this attribute, will be picked up by PackageCreator
    /// Category is shown in 'Package Creator' section as a folder for grouping package creator items
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public sealed class PackCategoryAttribute : Attribute
    {
        internal PackCategoryAttribute(string name)
        {
            this.Name = name;
            this.Label = string.Empty;
        }

        public PackCategoryAttribute(string name, string label)
        {
            this.Name = name;
            this.Label = label;
        }

        public string Name
        {
            get;
            private set;
        }

        private string Label
        {
            get;
            set;
        }

        public string CommonName
        {
            get;
            set;
        }

        public string[] AliasNames
        {
            get;
            set;
        }

        public string GetLabel()
        {
            if (string.IsNullOrEmpty(Label))
                return PackageCreatorFacade.GetLocalization(string.Format("{0}.Label", Name));
            return Label;
        }
    }

}
