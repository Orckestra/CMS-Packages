using System.Collections.Concurrent;

namespace C1PackageDevProvisioner
{
    public static class EventInfo
    {
        public readonly static ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();
    }
}
