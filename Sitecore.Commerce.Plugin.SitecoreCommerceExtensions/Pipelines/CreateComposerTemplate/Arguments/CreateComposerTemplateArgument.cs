using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;

namespace Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate.Arguments
{
    public class CreateComposerTemplateArgument : PipelineArgument
    {
        public string ComposerTemplateId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public CreateComposerTemplateArgument(string composerTemplateId)
        {
            Condition.Requires<string>(composerTemplateId).IsNotNullOrEmpty("The composer template identifier can not be null");
            ComposerTemplateId = composerTemplateId;
        }
    }
}
