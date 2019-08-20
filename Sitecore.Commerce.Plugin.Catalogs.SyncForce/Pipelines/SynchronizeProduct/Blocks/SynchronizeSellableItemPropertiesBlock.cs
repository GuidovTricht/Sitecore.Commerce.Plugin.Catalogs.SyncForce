using System;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Arguments;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Policies;
using Sitecore.Commerce.Plugin.Composer;
using Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate;
using Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Pipelines.CreateComposerTemplate.Arguments;
using Sitecore.Commerce.Plugin.Views;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeProduct.Blocks
{
    public class SynchronizeSellableItemPropertiesBlock : PipelineBlock<SynchronizeProductArgument, SynchronizeProductArgument, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IDoesEntityExistPipeline _doesEntityExistPipeline;
        private readonly ICreateComposerTemplatePipeline _createComposerTemplatePipeline;
        private readonly ComposerCommander _composerCommander;

        public SynchronizeSellableItemPropertiesBlock(IFindEntityPipeline findEntityPipeline, IPersistEntityPipeline persistEntityPipeline, IDoesEntityExistPipeline doesEntityExistPipeline, ICreateComposerTemplatePipeline createComposerTemplatePipeline, ComposerCommander composerCommander)
        {
            _findEntityPipeline = findEntityPipeline;
            _persistEntityPipeline = persistEntityPipeline;
            _doesEntityExistPipeline = doesEntityExistPipeline;
            _createComposerTemplatePipeline = createComposerTemplatePipeline;
            _composerCommander = composerCommander;
        }

        public override async Task<SynchronizeProductArgument> Run(SynchronizeProductArgument arg, CommercePipelineExecutionContext context)
        {
            var syncForcePolicy = context.CommerceContext.GetPolicy<SyncForceClientPolicy>();
            var sellableItem = arg.SellableItem;

            var commonComposerTemplate = await EnsureComposerTemplate(context.CommerceContext, syncForcePolicy.CommonComposerTemplateName);
            await UpdateSyncforceComposerTemplate(arg, context, sellableItem, commonComposerTemplate, syncForcePolicy);
            arg.SellableItem = (await _persistEntityPipeline.Run(new PersistEntityArgument(sellableItem), context.CommerceContext.PipelineContextOptions))?.Entity as SellableItem ?? sellableItem;

            //Replace item from list
            var oldItem = arg.SellableItems.FirstOrDefault(s => s.Id == arg.SellableItem.Id);
            arg.SellableItems.Remove(oldItem);
            arg.SellableItems.Add(arg.SellableItem);

            return arg;
        }

        private async Task<ComposerTemplate> EnsureComposerTemplate(CommerceContext context, string name)
        {
            if (await _doesEntityExistPipeline.Run(new FindEntityArgument(typeof(ComposerTemplate), $"{CommerceEntity.IdPrefix<ComposerTemplate>()}{name}"), context.PipelineContextOptions))
            {
                return await _findEntityPipeline.Run(new FindEntityArgument(typeof(ComposerTemplate), $"{CommerceEntity.IdPrefix<ComposerTemplate>()}{name}"), context.PipelineContextOptions) as ComposerTemplate;
            }

            return (await _createComposerTemplatePipeline.Run(new CreateComposerTemplateArgument(name) { Name = name, DisplayName = name },
                context.PipelineContextOptions))?.ComposerTemplate;
        }

        private async Task<ComposerTemplate> EnsureComposerTemplateProperty(SyncForceClientPolicy syncForcePolicy, ComposerTemplate template, CommerceContext context, EntityView childView, string name, string propertyType)
        {
            var entityViewComponent = template.GetComponent<EntityViewComponent>();
            if (childView.Properties.All(p => p.Name != name))
            {
                if (entityViewComponent.View.GetChildView(
                    it => String.Equals(it.Name, syncForcePolicy.CommonComposerTemplateName, StringComparison.InvariantCultureIgnoreCase)) is EntityView templateView)
                {
                    templateView.Properties.Add(new ViewProperty()
                    {
                        DisplayName = name,
                        IsHidden = false,
                        OriginalType = propertyType,
                        IsRequired = false,
                        IsReadOnly = false,
                        Name = name
                    });

                    template.SetComponent(entityViewComponent);

                    return (await _persistEntityPipeline.Run(new PersistEntityArgument(template),
                        context.PipelineContextOptions))?.Entity as ComposerTemplate;
                }
            }
            return template;
        }

        private async Task UpdateSyncforceComposerTemplate(SynchronizeProductArgument arg,
            CommercePipelineExecutionContext context, SellableItem sellableItem, ComposerTemplate commonComposerTemplate,
            SyncForceClientPolicy syncForcePolicy)
        {
            var sellableEntityViewComponent = sellableItem.GetComponent<EntityViewComponent>();
            if (sellableEntityViewComponent != null)
            {

                EntityView tempViewHolder = new EntityView();
                tempViewHolder.SetPropertyValue("Template", syncForcePolicy.CommonComposerTemplateName);
                tempViewHolder.GetProperty(it =>
                        String.Equals(it.Name, "Template", StringComparison.InvariantCultureIgnoreCase)).Value =
                    syncForcePolicy.CommonComposerTemplateName;

                EntityView templateView = !string.IsNullOrEmpty(tempViewHolder.ItemId) ? sellableItem.GetComponent<EntityViewComponent>().ChildViewWithItemId(tempViewHolder.ItemId) : sellableItem.GetComponent<EntityViewComponent>().View.ChildViews.OfType<EntityView>().FirstOrDefault<EntityView>();
                if (templateView == null || !sellableItem.GetComponent<EntityViewComponent>().HasChildViews((Func<EntityView, bool>)(p => p.Name.Equals(templateView.Name, StringComparison.OrdinalIgnoreCase))))
                {
                    await _composerCommander.AddChildViewFromTemplate(context.CommerceContext, tempViewHolder, sellableItem);
                }

                var composerView = sellableItem.GetComponent<EntityViewComponent>()?.View.ChildViews?.OfType<EntityView>()
                    ?.FirstOrDefault(v => v.Name == syncForcePolicy.CommonComposerTemplateName);

                if (composerView != null)
                {
                    //Add SyncForce timestamp
                    var timestampDateTime = DateTime.SpecifyKind(arg.MasterProduct.TimeStamp, DateTimeKind.Utc);
                    var timestampDateTimeOffset = (DateTimeOffset)timestampDateTime;
                    var timestampRawValue = timestampDateTimeOffset.ToString("yyyy-MM-dd'T'H:mm:ss.fffffffzzz");

                    //Ensure timestamp field on ComposerTemplate
                    await EnsureComposerTemplateProperty(syncForcePolicy, commonComposerTemplate, context.CommerceContext, composerView, "Timestamp", "System.DateTimeOffset");

                    if (composerView.ContainsProperty("Timestamp"))
                        composerView.SetPropertyValue("Timestamp", timestampRawValue);
                    else
                    {
                        composerView.Properties.Add(new ViewProperty()
                        {
                            DisplayName = "Timestamp",
                            RawValue = timestampRawValue,
                            IsHidden = false,
                            IsReadOnly = false,
                            IsRequired = false,
                            Name = "Timestamp",
                            OriginalType = "System.DateTimeOffset"
                        });
                    }
                }
            }
        }
    }
}
