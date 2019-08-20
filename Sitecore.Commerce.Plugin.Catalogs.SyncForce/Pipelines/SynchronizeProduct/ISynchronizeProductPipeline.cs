using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Arguments;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct
{
    public interface ISynchronizeProductPipeline : IPipeline<SynchronizeProductArgument, CatalogContentArgument, CommercePipelineExecutionContext>
    {
    }
}
