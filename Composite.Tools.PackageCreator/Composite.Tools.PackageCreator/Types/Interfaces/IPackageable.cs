namespace Composite.Tools.PackageCreator.Types
{
    /// <summary>
    /// Interface for populating <see cref="PackageCreator"/> with data
    /// </summary>
    public interface IPackageable
    {
        void Pack(PackageCreator creator);
    }
}
