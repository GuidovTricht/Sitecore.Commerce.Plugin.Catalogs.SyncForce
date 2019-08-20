using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Commands;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Arguments;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Services;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Blocks
{
    /// <summary>
    /// Downloads the Proposition object from SyncForce and adds it to the result arg.
    /// </summary>
    public class DownloadSyncForcePropositionNavigationBlock : PipelineBlock<SynchronizeCatalogArgument, SynchronizeCatalogArgument, CommercePipelineExecutionContext>
    {
        private readonly GetSyncForcePropositionNavigationCommand _getSyncForcePropositionNavigationCommand;

        public DownloadSyncForcePropositionNavigationBlock(GetSyncForcePropositionNavigationCommand getSyncForcePropositionNavigationCommand)
        {
            _getSyncForcePropositionNavigationCommand = getSyncForcePropositionNavigationCommand;
        }

        public override async Task<SynchronizeCatalogArgument> Run(SynchronizeCatalogArgument arg, CommercePipelineExecutionContext context)
        {
            //Get SyncForce proposition
            var propositionNavigation = await _getSyncForcePropositionNavigationCommand.Process(context.CommerceContext);
            Condition.Requires<Collection>(propositionNavigation?.Proposition).IsNotNull($"{this.Name}: SyncForce did not return a Proposition.");

            arg.Proposition = propositionNavigation?.Proposition;

            return arg;
        }
    }
}
