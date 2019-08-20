using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Blocks;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Blocks;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using ConfigureServiceApiBlock = Sitecore.Commerce.Plugin.Catalogs.SyncForce.ConfigureServiceApiBlock;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce
{
    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config

             .AddPipeline<ISynchronizeCatalogPipeline, SynchronizeCatalogPipeline>(
                    configure =>
                        {
                            configure
                                .Add<DownloadSyncForcePropositionNavigationBlock>()
                                .Add<GetOrCreateCatalogBlock>()
                                .Add<CreateBaseItemDefinitionsBlock>()
                                .Add<CreateAndUpdateCategoriesBlock>()
                                .Add<DownloadAllSyncForceProductsBlock>()
                                .Add<AddProductsToProductUpdatesListBlock>();
                        })

             .AddPipeline<ISynchronizeProductPipeline, SynchronizeProductPipeline>(
                 configure =>
                 {
                     configure
                         .Add<DownloadSyncForceProductBlock>()
                         .Add<GetOrCreateSellableItemBlock>()
                         .Add<SynchronizeSellableItemCatalogStructureBlock>()
                         .Add<SynchronizeSellableItemPropertiesBlock>()
                         .Add<SynchronizeSellableItemVariantsBlock>();
                 })

               .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}