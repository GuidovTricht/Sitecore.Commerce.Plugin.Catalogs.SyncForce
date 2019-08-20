using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Arguments;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct
{
    public class SynchronizeProductPipeline : CommercePipeline<SynchronizeProductArgument, CatalogContentArgument>, ISynchronizeProductPipeline
    {
        public SynchronizeProductPipeline(IPipelineConfiguration<ISynchronizeProductPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
