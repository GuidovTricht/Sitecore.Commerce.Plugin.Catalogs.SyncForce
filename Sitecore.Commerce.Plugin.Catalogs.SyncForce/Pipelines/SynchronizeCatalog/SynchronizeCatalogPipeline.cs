using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Arguments;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog
{
    public class SynchronizeCatalogPipeline : CommercePipeline<SynchronizeCatalogArgument, CatalogContentArgument>, ISynchronizeCatalogPipeline
    {
        public SynchronizeCatalogPipeline(IPipelineConfiguration<ISynchronizeCatalogPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
