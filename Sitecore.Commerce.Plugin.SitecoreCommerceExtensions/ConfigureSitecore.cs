using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate;
using Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate.Blocks;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Sitecore.Commerce.Plugin.SitecoreCommerceExtensions
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
            
             .AddPipeline<ICreateComposerTemplatePipeline, CreateComposerTemplatePipeline>(
                 configure =>
                 {
                     configure
                         .Add<CreateComposerTemplateBlock>();
                 })
            );

            services.RegisterAllCommands(assembly);
        }
    }
}