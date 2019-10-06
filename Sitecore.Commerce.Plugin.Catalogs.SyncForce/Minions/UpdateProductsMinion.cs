using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Arguments;
using Sitecore.Framework.Conditions;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Minions
{
    public class UpdateProductsMinion : Minion
    {
        private ISynchronizeProductPipeline _synchronizeProductPipeline;
        private IRemoveListEntitiesPipeline _removeListEntitiesPipeline;
        private IGetListsEntityIdsPipeline _getListsEntityIdsPipeline;

        public override void Initialize(IServiceProvider serviceProvider, MinionPolicy policy, CommerceContext globalContext)
        {
            base.Initialize(serviceProvider, policy, globalContext);
            _synchronizeProductPipeline = this.ResolvePipeline<ISynchronizeProductPipeline>();
            _removeListEntitiesPipeline = this.ResolvePipeline<IRemoveListEntitiesPipeline>();
            _getListsEntityIdsPipeline = this.ResolvePipeline<IGetListsEntityIdsPipeline>();
        }

        protected override async Task<MinionRunResultsModel> Execute()
        {
            long listCount = 0;
            try
            {
                listCount = await this.GetListCount(this.Policy.ListToWatch);
                this.Logger.LogInformation($"{this.Name}-Review List {this.Policy.ListToWatch}: Count:{listCount}");
                var lists = (await _getListsEntityIdsPipeline.Run(new GetListsEntityIdsArgument(new []{this.Policy.ListToWatch}), MinionContext.PipelineContextOptions));

                if (lists == null || !lists.Any())
                {
                    this.Logger.LogError($"{this.Name}: No ids were found in the list {this.Policy.ListToWatch}");
                    return new MinionRunResultsModel(false, 0, false);
                }

                string text = MinionContext.GetPolicy<ListNamePolicy>().ListName(this.Policy.ListToWatch.ToUpperInvariant());
                var list = lists[text];

                foreach (var id in list)
                {
                    this.Logger.LogDebug($"{this.Name}-Reviewing Pending Product Update: {id}");

                    var ids = id.Split('_');

                    var catalogId = ids[0];
                    var productId = Convert.ToInt32(ids[1]);
                    Condition.Requires(productId)
                        .IsGreaterThan(0, $"{this.Name}: could not convert ExternalProductId to int.");
                    try
                    {
                        await _synchronizeProductPipeline.Run(
                            new SynchronizeProductArgument { ExternalProductId = productId, CatalogId = catalogId.ToEntityId<Sitecore.Commerce.Plugin.Catalog.Catalog>() },
                            MinionContext.PipelineContextOptions);
                        this.Logger.LogInformation($"{this.Name}: Product with id {id} has been updated");

                        await _removeListEntitiesPipeline.Run(
                            new ListEntitiesArgument(new List<string>() { id }, this.Policy.ListToWatch),
                            MinionContext.PipelineContextOptions);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogError(ex, $"{this.Name}: Product with id {id} could not be updated");
                    }
                }
            }
            catch (Exception e)
            {
                this.Logger.LogError(e, "Error in UpdateProductsMinion");
                this.Environment.RemoveRunningMinion(this);
            }
            return new MinionRunResultsModel
            {
                ItemsProcessed = this.Policy.ItemsPerBatch,
                HasMoreItems = listCount > this.Policy.ItemsPerBatch
            };
        }
    }
}
