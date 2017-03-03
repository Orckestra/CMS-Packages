using System;

namespace Composite.Package.Server
{
    public class SubscriptionDescriptor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DetailsUrl { get; set; }
        public bool Purchasable { get; set; }
    }
}
