using System.Xml.Linq;
using Composite.Core.ResourceSystem;

namespace Composite.Tools.PackageCreator.Types
{
    /// <summary>
    /// Represents an item from a package creator configuration.
    /// Can be either deserialized from package configuration or created for a given entity token and then added to configuration.
    /// Derived class should have a public static "Create" method that creates instances of a class from entity tokens (see the example).
    /// </summary>
    /// <example>
    /// [PCCategory("xxxxxxx")]
    /// internal class PCDirectory : SimplePackageCreatorItem
    /// {
    ///   // ..
    ///   public static IEnumerable&lt;IPackageCreatorItem&gt; Create(EntityToken entityToken)
    ///   {
    ///       if (entityToken is xxxxEntityToken) { .... }
    ///   }
    /// }
    /// </example>
    public interface IPackageCreatorItem
    {
        string Id
        {
            get;
        }

        string ActionLabel
        {
            get;
        }

        string ActionToolTip
        {
            get;
        }

        ResourceHandle ActionIcon
        {
            get;
        }

        /// <summary>
        /// Icon that is used for showing the package creator item in 'Package Creator' perspective
        /// </summary>
        ResourceHandle ItemIcon
        {
            get;
        }

        string CategoryName { get; }

        string[] CategoryAllNames { get; }

        /// <summary>
        /// Gets the label for console action.
        /// </summary>
        /// <returns></returns>
        string GetLabel();

        /// <summary>
        /// Adds the package creator item to configuration.
        /// </summary>
        /// <param name="config">The config.</param>
        void AddToConfiguration(XElement config);

        /// <summary>
        /// Removes the package creator item from configuration.
        /// </summary>
        /// <param name="config">The config.</param>
        void RemoveFromConfiguration(XElement config);
    }
}
