namespace Composite.Tools.PackageCreator.Types
{
    /// <summary>
    /// Interface for populating <see cref="PackageCreator"/> with data
    /// </summary>
    public interface IPack
    {
        void Pack(PackageCreator creator);
    }
}
