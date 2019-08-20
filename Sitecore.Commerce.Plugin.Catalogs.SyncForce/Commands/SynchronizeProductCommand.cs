using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Arguments;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Commands
{
    public class SynchronizeProductCommand : CommerceCommand
    {
        private readonly ISynchronizeProductPipeline _synchronizeProductPipeline;

        public SynchronizeProductCommand(ISynchronizeProductPipeline synchronizeProductPipeline)
        {
            _synchronizeProductPipeline = synchronizeProductPipeline;
        }

        public virtual async Task<CatalogContentArgument> Process(CommerceContext commerceContext, string catalogId = "", string productId = "", string entityId = "", int externalProductId = 0)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var arg = new SynchronizeProductArgument
                {
                    CatalogId = catalogId,
                    ProductId = productId,
                    EntityId = entityId,
                    ExternalProductId = externalProductId
                };
                return await _synchronizeProductPipeline.Run(arg, new CommercePipelineExecutionContextOptions(commerceContext));
            }
        }
    }
}
