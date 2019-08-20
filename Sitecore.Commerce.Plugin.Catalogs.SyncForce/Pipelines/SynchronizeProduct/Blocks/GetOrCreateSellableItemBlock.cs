using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Arguments;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Policies;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Blocks
{
    public class GetOrCreateSellableItemBlock : PipelineBlock<SynchronizeProductArgument, SynchronizeProductArgument, CommercePipelineExecutionContext>
    {
        private readonly IGetSellableItemPipeline _getSellableItemPipeline;
        private readonly ICreateSellableItemPipeline _createSellableItemPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IDoesEntityExistPipeline _doesEntityExistPipeline;

        public GetOrCreateSellableItemBlock(IGetSellableItemPipeline getSellableItemPipeline, ICreateSellableItemPipeline createSellableItemPipeline, IPersistEntityPipeline persistEntityPipeline, IDoesEntityExistPipeline doesEntityExistPipeline)
        {
            this._getSellableItemPipeline = getSellableItemPipeline;
            this._createSellableItemPipeline = createSellableItemPipeline;
            _persistEntityPipeline = persistEntityPipeline;
            _doesEntityExistPipeline = doesEntityExistPipeline;
        }

        /// <summary>
        /// Ensures the SellableItem exists and creates it if it doesn't.
        /// Adds/updates the product identifiers and Brand.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <param name="context">The context.</param>
        /// <returns>The arg with an updated SellableItem property.</returns>
        public override async Task<SynchronizeProductArgument> Run(SynchronizeProductArgument arg, CommercePipelineExecutionContext context)
        {
            var syncForceClientPolicy = context.GetPolicy<SyncForceClientPolicy>();
            var validMasterCode = arg.MasterProduct.MasterCode.ProposeValidId();
            var sellableItemId = $"{CommerceEntity.IdPrefix<SellableItem>()}{validMasterCode}";
            SellableItem sellableItem = null;

            if (await _doesEntityExistPipeline.Run(new FindEntityArgument(typeof(SellableItem), sellableItemId),
                context.CommerceContext.PipelineContextOptions))
            {
                sellableItem = await _getSellableItemPipeline.Run(new ProductArgument { CatalogName = "", ProductId = sellableItemId }, context.CommerceContext.PipelineContextOptions);
            }
            else
            {
                var createResult = (await _createSellableItemPipeline.Run(new CreateSellableItemArgument(arg.MasterProduct.MasterCode.ProposeValidId(), arg.MasterProduct.MasterCode, arg.MasterProduct.MasterCode, string.Empty), context.CommerceContext.PipelineContextOptions));
                sellableItem = createResult?.SellableItems?.FirstOrDefault(s => s.Id == sellableItemId);

                Condition.Requires<SellableItem>(sellableItem).IsNotNull($"{this.Name}: The SellableItem could not be created.");
            }

            //Sitecore should have set this to true, issue created #514238
            sellableItem.IsPersisted = true;

            //Add Base properties
            sellableItem.Brand = arg.MasterProduct.Brand.Name;

            //Clear Tags
            sellableItem.Tags.Clear();

            //Add identifiers
            var identifiersComponent = sellableItem.GetComponent<IdentifiersComponent>();
            identifiersComponent.SKU = arg.MasterProduct.MasterCode;
            if (!identifiersComponent.CustomId.Any(i => i.Key.Equals(syncForceClientPolicy.CustomIdentifierKey)))
                identifiersComponent.CustomId.Add(new Models.CustomIdentifier(syncForceClientPolicy.CustomIdentifierKey, arg.MasterProduct.Id.ToString()));
            sellableItem.SetComponent(identifiersComponent);

            sellableItem = (await _persistEntityPipeline.Run(new PersistEntityArgument(sellableItem), context.CommerceContext.PipelineContextOptions))?.Entity as SellableItem ?? sellableItem;

            arg.SellableItem = sellableItem;
            arg.SellableItems?.Add(sellableItem);

            return arg;
        }
    }
}
