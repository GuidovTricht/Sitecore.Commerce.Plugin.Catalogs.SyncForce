using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Commands;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Arguments;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Blocks
{
    public class DownloadSyncForceProductBlock : PipelineBlock<SynchronizeProductArgument, SynchronizeProductArgument, CommercePipelineExecutionContext>
    {
        private readonly GetSyncForceProductCommand _getSyncForceProductCommand;

        public DownloadSyncForceProductBlock(GetSyncForceProductCommand getSyncForceProductCommand)
        {
            _getSyncForceProductCommand = getSyncForceProductCommand;
        }

        public override async Task<SynchronizeProductArgument> Run(SynchronizeProductArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg.ExternalProductId).IsNotNull($"{this.Name}: an ExternalProductId must be specified.");
            Condition.Requires(arg.ExternalProductId).IsGreaterThan(0, $"{this.Name}: an ExternalProductId must be specified and greater than 0.");

            var result = new SynchronizeProductArgument
            {
                ExternalProductId = arg.ExternalProductId,
                CatalogId = arg.CatalogId
            };

            var product = await _getSyncForceProductCommand.Process(context.CommerceContext, arg.ExternalProductId);
            Condition.Requires(product?.GetFullProductResult?.MasterProduct).IsNotNull($"{this.Name}: an MasterProduct was not returned by SyncForce for MasterProductId '{arg.ExternalProductId}'.");

            result.MasterProduct = product.GetFullProductResult.MasterProduct;
            return result;
        }
    }
}
