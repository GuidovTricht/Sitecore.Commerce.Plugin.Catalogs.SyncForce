using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Services;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Arguments
{
    public class SynchronizeProductArgument : CatalogContentArgument
    {
        public string ProductId { get; set; }
        public string EntityId { get; set; }
        public int ExternalProductId { get; set; }
        public string CatalogId { get; set; }
        public SellableItem SellableItem { get; set; }
        public MasterProduct MasterProduct { get; set; }
    }
}
