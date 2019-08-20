using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Arguments;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Policies;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Blocks
{
    public class SynchronizeSellableItemCatalogStructureBlock : PipelineBlock<SynchronizeProductArgument, SynchronizeProductArgument, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;
        private readonly IGetCategoriesPipeline _getCategoriesPipeline;
        private readonly IAssociateSellableItemToParentPipeline _associateSellableItemToParentPipeline;

        public SynchronizeSellableItemCatalogStructureBlock(IFindEntityPipeline findEntityPipeline, IGetCategoriesPipeline getCategoriesPipeline, IAssociateSellableItemToParentPipeline associateSellableItemToParentPipeline)
        {
            _findEntityPipeline = findEntityPipeline;
            _getCategoriesPipeline = getCategoriesPipeline;
            _associateSellableItemToParentPipeline = associateSellableItemToParentPipeline;
        }

        /// <summary>
        /// Gets all categories and associates the SellableItem to the correct category based on the SyncForce id in the custom Identifiers of the categories.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <param name="context">The context.</param>
        /// <returns>The arg with an updated SellableItem property.</returns>
        public override async Task<SynchronizeProductArgument> Run(SynchronizeProductArgument arg, CommercePipelineExecutionContext context)
        {
            var syncForceClientPolicy = context.GetPolicy<SyncForceClientPolicy>();
            var catalog = await _findEntityPipeline.Run(new FindEntityArgument(typeof(Sitecore.Commerce.Plugin.Catalog.Catalog), arg.CatalogId.ToEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>()), context.CommerceContext.PipelineContextOptions) as Sitecore.Commerce.Plugin.Catalog.Catalog;

            if (catalog == null)
            {
                context.Logger.LogError($"{this.Name}: The Catalog with id {arg.CatalogId} could not be found.");
                return arg;
            }

            //Get all categories.
            var allCategories = await _getCategoriesPipeline.Run(new GetCategoriesArgument(catalog.Name), context.CommerceContext.PipelineContextOptions);
            arg.Categories.AddRange(allCategories);

            //Get all SyncForce category id's to associate with.
            var categoryIds = arg.MasterProduct.ProductVariants.FirstOrDefault()?.ProductPropositionCategories.Select(c => c.Id);
            if (categoryIds == null)
                return arg;

            //For each SyncForce categoryId find the correct Sitecore Commerce category and associate the SellableItem with it.
            foreach (var categoryId in categoryIds)
            {
                var category = arg.Categories.FirstOrDefault(c => c.GetComponent<IdentifiersComponent>().CustomId.FirstOrDefault(i => i.Key.Equals(syncForceClientPolicy.CustomIdentifierKey))?.Value == categoryId.ToString());
                if (category != null && !string.IsNullOrEmpty(category.Id))
                {
                    await _associateSellableItemToParentPipeline.Run(
                        new CatalogReferenceArgument(arg.CatalogId, category.Id, arg.SellableItem.Id),
                        context.CommerceContext.PipelineContextOptions);
                    arg.SellableItem = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), arg.SellableItem.Id, shouldCreate: false), context.CommerceContext.PipelineContextOptions) as SellableItem ?? arg.SellableItem;
                }
            }

            return arg;
        }
    }
}
