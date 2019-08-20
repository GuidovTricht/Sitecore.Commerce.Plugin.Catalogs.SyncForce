using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Arguments;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog
{
    public interface ISynchronizeCatalogPipeline : IPipeline<SynchronizeCatalogArgument, CatalogContentArgument, CommercePipelineExecutionContext>
    {
    }
}
