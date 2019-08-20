using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate.Arguments;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate
{
    public class CreateComposerTemplatePipeline : CommercePipeline<CreateComposerTemplateArgument, ComposerTemplateContentArgument>, ICreateComposerTemplatePipeline
    {
        public CreateComposerTemplatePipeline(IPipelineConfiguration<ICreateComposerTemplatePipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
