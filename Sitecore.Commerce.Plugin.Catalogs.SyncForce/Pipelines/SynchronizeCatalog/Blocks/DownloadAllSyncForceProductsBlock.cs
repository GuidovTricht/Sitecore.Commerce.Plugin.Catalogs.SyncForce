using System;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Commands;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Arguments;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Services;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Blocks
{
    public class DownloadAllSyncForceProductsBlock : PipelineBlock<SynchronizeCatalogArgument, SynchronizeCatalogArgument, CommercePipelineExecutionContext>
    {
        private readonly GetAllSyncForceProductsCommand _getAllSyncForceProductsCommand;

        public DownloadAllSyncForceProductsBlock(GetAllSyncForceProductsCommand getAllSyncForceProductsCommand)
        {
            _getAllSyncForceProductsCommand = getAllSyncForceProductsCommand;
        }

        public override async Task<SynchronizeCatalogArgument> Run(SynchronizeCatalogArgument arg, CommercePipelineExecutionContext context)
        {
            //Get SyncForce products
            var allProducts = await _getAllSyncForceProductsCommand.Process(context.CommerceContext);
            Condition.Requires<MasterProduct[]>(allProducts?.Proposition?.MasterProducts).IsNotNull($"{this.Name}: SyncForce did not return any MasterProducts.");

            arg.MasterProducts = allProducts?.Proposition?.MasterProducts.ToArray();

            return arg;
        }
    }
}
