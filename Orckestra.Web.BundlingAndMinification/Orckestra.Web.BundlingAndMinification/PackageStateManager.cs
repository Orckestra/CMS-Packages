using System.Threading;

namespace Orckestra.Web.BundlingAndMinification
{
    /// <summary>
    /// In case of critical situations turnning off the package, the site will be not optimized but still working
    /// </summary>
    public static class PackageStateManager
    {
        private static readonly ReaderWriterLockSlim _slimLock = new ReaderWriterLockSlim();
        private static bool _isCriticalState = false;

        internal static void SetCriticalState()
        {
            _slimLock.EnterWriteLock();
            _isCriticalState = true;
            _slimLock.ExitWriteLock();
        }

        internal static bool IsCriticalState()
        {
            _slimLock.EnterReadLock();
            var criticalStatusValue = _isCriticalState;
            _slimLock.ExitReadLock();
            return criticalStatusValue;
        }
    }
}
