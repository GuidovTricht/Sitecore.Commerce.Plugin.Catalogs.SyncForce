using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Arguments;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Blocks
{
    public class AddProductsToProductUpdatesListBlock : PipelineBlock<SynchronizeCatalogArgument, CatalogContentArgument, CommercePipelineExecutionContext>
    {
        private readonly IAddListEntitiesPipeline _addListEntitiesPipeline;

        public AddProductsToProductUpdatesListBlock(IAddListEntitiesPipeline addListEntitiesPipeline)
        {
            _addListEntitiesPipeline = addListEntitiesPipeline;
        }

        public override async Task<CatalogContentArgument> Run(SynchronizeCatalogArgument arg, CommercePipelineExecutionContext context)
        {
            var ids = arg.MasterProducts.Select(p => $"{arg.CatalogId}_{p.Id.ToString()}");
            var addArg = new ListEntitiesArgument(ids, "ProductUpdatesList");
            await _addListEntitiesPipeline.Run(addArg, context);

            return arg;
        }
    }
}
