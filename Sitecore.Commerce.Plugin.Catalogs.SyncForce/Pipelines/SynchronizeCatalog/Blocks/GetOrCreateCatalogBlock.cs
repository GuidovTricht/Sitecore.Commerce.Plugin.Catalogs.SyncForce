using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Arguments;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Blocks
{
    public class GetOrCreateCatalogBlock : PipelineBlock<SynchronizeCatalogArgument, SynchronizeCatalogArgument, CommercePipelineExecutionContext>
    {
        private readonly IGetCatalogPipeline _getCatalogPipeline;
        private readonly ICreateCatalogPipeline _createCatalogPipeline;
        private readonly IDoesEntityExistPipeline _doesEntityExistPipeline;

        public GetOrCreateCatalogBlock(IGetCatalogPipeline getCatalogPipeline, ICreateCatalogPipeline createCatalogPipeline, IDoesEntityExistPipeline doesEntityExistPipeline)
        {
            _getCatalogPipeline = getCatalogPipeline;
            _createCatalogPipeline = createCatalogPipeline;
            _doesEntityExistPipeline = doesEntityExistPipeline;
        }

        public override async Task<SynchronizeCatalogArgument> Run(SynchronizeCatalogArgument arg, CommercePipelineExecutionContext context)
        {
            Sitecore.Commerce.Plugin.Catalog.Catalog catalog = null;
            var catalogId = arg.Proposition.Name.ProposeValidId()
                .EnsurePrefix(CommerceEntity.IdPrefix<Sitecore.Commerce.Plugin.Catalog.Catalog>());
            if (await _doesEntityExistPipeline.Run(
                new FindEntityArgument(typeof(Sitecore.Commerce.Plugin.Catalog.Catalog), catalogId), context.CommerceContext.PipelineContextOptions))
            {
                catalog = await _getCatalogPipeline.Run(new GetCatalogArgument(arg.Proposition.Name.ProposeValidId()), context.CommerceContext.PipelineContextOptions);
            }
            else
            {
                var createResult = await _createCatalogPipeline.Run(new CreateCatalogArgument(arg.Proposition.Name.ProposeValidId(), arg.Proposition.Name), context.CommerceContext.PipelineContextOptions);
                catalog = createResult?.Catalog;
            }

            Condition.Requires<Sitecore.Commerce.Plugin.Catalog.Catalog>(catalog).IsNotNull($"{this.Name}: The Catalog could not be created.");

            arg.CatalogId = catalog?.Id;
            arg.Catalog = catalog;

            return arg;
        }
    }
}
