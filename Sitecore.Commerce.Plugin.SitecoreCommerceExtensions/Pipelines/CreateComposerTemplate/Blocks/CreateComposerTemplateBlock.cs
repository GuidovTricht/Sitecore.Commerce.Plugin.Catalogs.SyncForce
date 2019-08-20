using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Composer;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Helpers;
using Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate.Arguments;
using Sitecore.Commerce.Plugin.Views;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate.Blocks
{
    [PipelineDisplayName("Composer.block.createcomposertemplate")]
    public class CreateComposerTemplateBlock : PipelineBlock<CreateComposerTemplateArgument, ComposerTemplateContentArgument, CommercePipelineExecutionContext>
    {
        private readonly IDoesEntityExistPipeline _doesEntityExistPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        public CreateComposerTemplateBlock(IDoesEntityExistPipeline doesEntityExistPipeline, IPersistEntityPipeline persistEntityPipeline)
        {
            _doesEntityExistPipeline = doesEntityExistPipeline;
            _persistEntityPipeline = persistEntityPipeline;
        }

        public override async Task<ComposerTemplateContentArgument> Run(CreateComposerTemplateArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires<CreateComposerTemplateArgument>(arg).IsNotNull<CreateComposerTemplateArgument>($"{this.Name}: The argument can not be null");
            var composerTemplateId = $"{CommerceEntity.IdPrefix<ComposerTemplate>()}{arg.ComposerTemplateId}";

            var errorCodes = context.GetPolicy<KnownResultCodes>();
            if (await _doesEntityExistPipeline.Run(new FindEntityArgument(typeof(ComposerTemplate), composerTemplateId), context.CommerceContext.PipelineContextOptions))
            {
                var errorMessage = await context.CommerceContext.AddMessage(errorCodes.ValidationError,
                    "NameAlreadyInUse", new object[1] { arg.ComposerTemplateId },
                    $"Name '{arg.ComposerTemplateId}' is already in use.");
                return new ComposerTemplateContentArgument();
            }

            var composerTemplate = new ComposerTemplate(composerTemplateId)
            {
                Name = !string.IsNullOrEmpty(arg.Name) ? arg.Name : arg.ComposerTemplateId,
                DisplayName = arg.DisplayName,
                FriendlyId = arg.ComposerTemplateId
            };
            composerTemplate.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<ComposerTemplate>());

            var entityView = CreateEntityView(context.CommerceContext, arg.ComposerTemplateId, arg.Name, arg.DisplayName);
            if (entityView != null)
                composerTemplate.GetComponent<EntityViewComponent>().AddChildView(entityView);

            var persistResult = await _persistEntityPipeline.Run(new PersistEntityArgument(composerTemplate), context.CommerceContext.PipelineContextOptions);

            return new ComposerTemplateContentArgument() { ComposerTemplate = persistResult?.Entity as ComposerTemplate };
        }

        private EntityView CreateEntityView(CommerceContext context, string id, string name, string displayName = "")
        {
            var entityView = new EntityView
            {
                Name = name,
                DisplayName = displayName,
                Icon = "piece",
                DisplayRank = 0,
                ItemId = $"Composer-{IdHelper.ToUniqueId(id):N}"
            };

            return entityView;
        }
    }
}
