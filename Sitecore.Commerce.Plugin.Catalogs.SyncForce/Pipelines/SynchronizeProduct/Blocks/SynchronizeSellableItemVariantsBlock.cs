using System;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Arguments;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Blocks
{
    public class SynchronizeSellableItemVariantsBlock : PipelineBlock<SynchronizeProductArgument, SynchronizeProductArgument, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        public SynchronizeSellableItemVariantsBlock(IPersistEntityPipeline persistEntityPipeline)
        {
            _persistEntityPipeline = persistEntityPipeline;
        }

        public override async Task<SynchronizeProductArgument> Run(SynchronizeProductArgument arg, CommercePipelineExecutionContext context)
        {
            var sellableItem = arg.SellableItem;
            var variantsComponent = sellableItem.GetComponent<ItemVariationsComponent>();
            variantsComponent.ChildComponents.Clear();

            foreach (var variant in arg.MasterProduct.ProductVariants)
            {
                var displayName = variant?.Values?.FirstOrDefault(f => f.Language == "en")?.Name ??
                                  variant?.Values?.FirstOrDefault()?.Name ?? string.Empty;

                var variationItem = new ItemVariationComponent
                {
                    Id = variant.ArticleNr.ProposeValidId(),
                    Name = variant.ArticleNr,
                    DisplayName = displayName
                };

                //Add identifiers
                var identifiersComponent = variationItem.GetComponent<IdentifiersComponent>();
                identifiersComponent.SKU = variant.ArticleNr;
                identifiersComponent.gtin13 = variant.GTIN;
                variationItem.SetComponent(identifiersComponent);

                variantsComponent.ChildComponents.Add(variationItem);
            }

            arg.SellableItem = (await _persistEntityPipeline.Run(new PersistEntityArgument(sellableItem),
                                   context.CommerceContext.PipelineContextOptions))?.Entity as SellableItem ?? sellableItem;
            return arg;
        }
    }
}
