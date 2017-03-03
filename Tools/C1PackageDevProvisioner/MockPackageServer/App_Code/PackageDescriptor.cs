using System;
using System.Collections.Generic;

namespace Composite.Package.Server
{
    public class PackageDescriptor
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public string Name { get; set; }
        public string PackageVersion { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string TechicalDetails { get; set; }
        public string ConsoleBrowserUrl { get; set; }
        public string ReadMoreUrl { get; set; }
        public bool IsTrial { get; set; }
        public int? TrialPeriodDays { get; set; }
        public bool IsFree { get; set; }
        public bool InstallationRequireLicenseFileUpdate { get; set; }
        public decimal PriceAmmount { get; set; }
        public string PriceCurrency { get; set; }
        public bool UpgradeAgreementMandatory { get; set; }
        public string MinCompositeVersionSupported { get; set; }
        public string MaxCompositeVersionSupported { get; set; }
        public List<PackageReference> RequiredPackages { get; set; }
        public Guid EulaId { get; set; }
        public string PackageFileDownloadUrl { get; set; }
        public Guid LicenseId { get; set; }
        public string Culture { get; set; }
        public Guid UserId { get; set; }
        public bool Online { get; set; }
        public bool SubscriptionOnly { get; set; }
        public List<SubscriptionDescriptor> Subscriptions { get; set; }
    }
}
