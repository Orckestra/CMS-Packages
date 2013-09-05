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
        static private IEnumerable<PermissionType> _permissionTypes = new PermissionType[] { PermissionType.Administrate };

        public override bool IgnoreEntityTokenLocking
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionTypes; }
        }

        public PackageCreatorActionToken(Type type)
        {
            this._categoryName = type.GetCategoryAtribute().Name;
        }

        public PackageCreatorActionToken(string categoryName)
        {
            this._categoryName = categoryName;
        }
        public PackageCreatorActionToken(string categoryName, string name)
        {
            this._categoryName = categoryName;
            this._name = name;
        }

        public override string Serialize()
        {
            StringBuilder sb = new StringBuilder();

            StringConversionServices.SerializeKeyValuePair<string>(sb, "_categoryName", _categoryName);
            StringConversionServices.SerializeKeyValuePair<string>(sb, "_name", _name);

            return sb.ToString();
        }

        private string _categoryName;
        public string CategoryName
        {
            get
            {
                return _categoryName;
            }
        }

        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public static ActionToken Deserialize(string serializedData)
        {
            Dictionary<string, string> dic = StringConversionServices.ParseKeyValueCollection(serializedData);

            string categoryName = StringConversionServices.DeserializeValueString(dic["_categoryName"]);
            string name = StringConversionServices.DeserializeValueString(dic["_name"]);

            return new PackageCreatorActionToken(categoryName, name);
        }
    }
}
