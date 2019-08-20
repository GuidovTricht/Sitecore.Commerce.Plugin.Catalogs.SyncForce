using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Services;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Arguments
{
    public class SynchronizeCatalogArgument : CatalogContentArgument
    {
        public string CatalogId { get; set; }
        public MasterProduct[] MasterProducts { get; set; }
        public Collection Proposition { get; set; }
    }
}
