using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate.Arguments;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate
{
    [PipelineDisplayName("Composer.pipeline.createcomposertemplate")]
    public interface ICreateComposerTemplatePipeline : IPipeline<CreateComposerTemplateArgument, ComposerTemplateContentArgument, CommercePipelineExecutionContext>
    {
    }
}
