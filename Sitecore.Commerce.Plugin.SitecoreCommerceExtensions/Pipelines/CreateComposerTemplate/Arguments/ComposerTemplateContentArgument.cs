using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Composer;

namespace Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate.Arguments
{
    public class ComposerTemplateContentArgument : PipelineArgument
    {
        public ComposerTemplate ComposerTemplate { get; set; }
    }
}
