using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Core.PackageSystem;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.Data;
using Composite.Data.Types;
using Composite.Plugins.Elements.ElementProviders.VirtualElementProvider;

namespace Orckestra.Search.Package
{
    public class AddDefaultUserGroupPermissionsInstaller: BasePackageFragmentInstaller
    {
        public override IEnumerable<PackageFragmentValidationResult> Validate()
        {
            return Enumerable.Empty<PackageFragmentValidationResult>();
        }

        public override IEnumerable<XElement> Install()
        {
            using (var conn = new DataConnection())
            {
                var administratorUserGroup = new Guid("a313fdae-232e-4997-9a21-264649dff307");
                var developerUserGroup = new Guid("0dd7fdb9-dd4b-4990-a0ec-b42cca61e39c");
                var editorUserGroup = new Guid("c3d9049e-f95e-4375-a3fa-af3ee9f40606");

                var userGroupsIds = new [] { administratorUserGroup, developerUserGroup, editorUserGroup };

                var spEntityToken = new VirtualElementProviderEntityToken("VirtualElementProvider", "SearchPerspective");

                foreach (var userGroupId in userGroupsIds)
                {
                    if (conn.Get<IUserGroup>().Any(ug => ug.Id == userGroupId))
                    {
                        var tokens = UserGroupPerspectiveFacade.GetEntityTokens(userGroupId);
                        if (!tokens.Any(token => token.Equals(spEntityToken)))
                        {
                            var perspectiveAccess = conn.CreateNew<IUserGroupActivePerspective>();
                            perspectiveAccess.Id = Guid.NewGuid();
                            perspectiveAccess.SerializedEntityToken = EntityTokenSerializer.Serialize(spEntityToken);
                            perspectiveAccess.UserGroupId = userGroupId;

                            conn.Add(perspectiveAccess);
                        }
                    }
                }
            }

            return Enumerable.Empty<XElement>();
        }
    }
}
