using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Arguments;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Commands
{
    public class SynchronizeCatalogCommand : CommerceCommand
    {
        private readonly ISynchronizeCatalogPipeline _synchronizeCatalogPipeline;

        public SynchronizeCatalogCommand(ISynchronizeCatalogPipeline synchronizeCatalogPipeline, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._synchronizeCatalogPipeline = synchronizeCatalogPipeline;
        }

        public virtual async Task<CatalogContentArgument> Process(CommerceContext commerceContext, string catalogId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var arg = new SynchronizeCatalogArgument
                {
                    CatalogId = catalogId
                };
                return await _synchronizeCatalogPipeline.Run(arg, new CommercePipelineExecutionContextOptions(commerceContext));
            }
        }
    }
}
