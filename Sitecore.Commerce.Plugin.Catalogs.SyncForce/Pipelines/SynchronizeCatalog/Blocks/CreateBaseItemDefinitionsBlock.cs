using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Arguments;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Blocks
{
    /// <summary>
    /// Associate (and create) base product item definitions to the Catalog
    /// </summary>
    public class CreateBaseItemDefinitionsBlock : PipelineBlock<SynchronizeCatalogArgument, SynchronizeCatalogArgument, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        public CreateBaseItemDefinitionsBlock(IPersistEntityPipeline persistEntityPipeline)
        {
            _persistEntityPipeline = persistEntityPipeline;
        }

        public override async Task<SynchronizeCatalogArgument> Run(SynchronizeCatalogArgument arg, CommercePipelineExecutionContext context)
        {
            if (arg.Catalog != null)
            {
                var itemDefinitionsComponent = arg.Catalog.GetComponent<ItemDefinitionsComponent>();
                itemDefinitionsComponent.AddDefinitions(new List<string>() { "BaseProduct" });
                arg.Catalog.SetComponent(itemDefinitionsComponent);

                arg.Catalog = (await _persistEntityPipeline.Run(new PersistEntityArgument(arg.Catalog), context.CommerceContext.PipelineContextOptions))?.Entity as Sitecore.Commerce.Plugin.Catalog.Catalog;
            }

            return arg;
        }
    }
}
