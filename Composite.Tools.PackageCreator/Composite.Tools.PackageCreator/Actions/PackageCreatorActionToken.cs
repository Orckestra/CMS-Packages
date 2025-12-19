using System;
using System.Collections.Generic;
using System.Text;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Core.Serialization;

namespace Composite.Tools.PackageCreator.Actions
{
    [ActionExecutor(typeof(PackageCreatorElementProviderActionExecutor))]
    public class PackageCreatorActionToken : ActionToken
    {
        private static readonly IEnumerable<PermissionType> _permissionTypes = new[]
            { PermissionType.Administrate, PermissionType.Configure };

        public PackageCreatorActionToken(Type type)
        {
            CategoryName = type.GetCategoryAttribute().Name;
        }

        public PackageCreatorActionToken(string categoryName)
        {
            CategoryName = categoryName;
        }

        public PackageCreatorActionToken(string categoryName, string name)
        {
            CategoryName = categoryName;
            Name = name;
        }

        public override bool IgnoreEntityTokenLocking => true;

        public override IEnumerable<PermissionType> PermissionTypes => _permissionTypes;

        public string CategoryName { get; }
        public string Name { get; } = "";

        public override string Serialize()
        {
            var sb = new StringBuilder();

            StringConversionServices.SerializeKeyValuePair<string>(sb, "_categoryName", CategoryName);
            StringConversionServices.SerializeKeyValuePair<string>(sb, "_name", Name);

            return sb.ToString();
        }

        public static ActionToken Deserialize(string serializedData)
        {
            var dic = StringConversionServices.ParseKeyValueCollection(serializedData);

            var categoryName = StringConversionServices.DeserializeValueString(dic["_categoryName"]);
            var name = StringConversionServices.DeserializeValueString(dic["_name"]);

            return new PackageCreatorActionToken(categoryName, name);
        }
    }
}