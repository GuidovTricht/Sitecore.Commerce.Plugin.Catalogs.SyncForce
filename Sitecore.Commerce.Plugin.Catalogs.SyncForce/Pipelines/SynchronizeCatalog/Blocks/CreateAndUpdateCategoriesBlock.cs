using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Components;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Arguments;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Policies;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Services;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Pipelines.SynchronizeCatalog.Blocks
{
    /// <summary>
    /// Creates and updates all the categories present in the SyncForce Proposition structure.
    /// </summary>
    public class CreateAndUpdateCategoriesBlock : PipelineBlock<SynchronizeCatalogArgument, SynchronizeCatalogArgument,
        CommercePipelineExecutionContext>
    {
        private readonly IGetCategoryPipeline _getCategoryPipeline;
        private readonly ICreateCategoryPipeline _createCategoryPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;
        private readonly IAssociateCategoryToParentPipeline _associateCategoryToParentPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IDoesEntityExistPipeline _doesEntityExistPipeline;

        public CreateAndUpdateCategoriesBlock(IGetCategoryPipeline getCategoryPipeline,
            ICreateCategoryPipeline createCategoryPipeline, IFindEntityPipeline findEntityPipeline,
            IAssociateCategoryToParentPipeline associateCategoryToParentPipeline,
            IPersistEntityPipeline persistEntityPipeline, IDoesEntityExistPipeline doesEntityExistPipeline)
        {
            _getCategoryPipeline = getCategoryPipeline;
            _createCategoryPipeline = createCategoryPipeline;
            _findEntityPipeline = findEntityPipeline;
            _associateCategoryToParentPipeline = associateCategoryToParentPipeline;
            _persistEntityPipeline = persistEntityPipeline;
            _doesEntityExistPipeline = doesEntityExistPipeline;
        }

        /// <summary>
        /// Loops through all the Categories in the Proposition and ensures the existence of these Categories in XC.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <param name="context">The context.</param>
        /// <returns>Returns the arg including all changed/created Categories.</returns>
        public override async Task<SynchronizeCatalogArgument> Run(SynchronizeCatalogArgument arg,
            CommercePipelineExecutionContext context)
        {
            var syncForceClientPolicy = context.GetPolicy<SyncForceClientPolicy>();
            Condition.Requires(arg?.Proposition?.ProductPropositionCategories)
                .IsNotNull($"{this.Name}: There are no categories in the pipeline arguments.");

            foreach (var c in arg.Proposition.ProductPropositionCategories)
            {
                arg.Categories.AddRange(await EnsureCategory(c, arg.CatalogId, arg.Catalog.Name, syncForceClientPolicy,
                    context.CommerceContext, true));
            }

            return arg;
        }

        /// <summary>
        /// Ensures the existence of the Category.
        /// If it doesn't exist it creates the Category, adds DisplayNames, adds identifiers and associates the Category with the parent Category.
        /// </summary>
        /// <param name="propositionCategory"></param>
        /// <param name="catalogId"></param>
        /// <param name="catalogName"></param>
        /// <param name="policy"></param>
        /// <param name="context"></param>
        /// <param name="associateWithCatalog"></param>
        /// <returns></returns>
        private async Task<List<Category>> EnsureCategory(CollectionFolder propositionCategory, string catalogId,
            string catalogName, SyncForceClientPolicy policy, CommerceContext context,
            bool associateWithCatalog = false)
        {
            var result = new List<Category>();

            var categoryName = propositionCategory.Values.FirstOrDefault(l => l.Language == policy.DefaultLanguage)
                ?.Name;
            //Added sort index to category name to make sure sorting is done right, instead of done on alphabetical order.
            var categoryId =
                $"{CommerceEntity.IdPrefix<Category>()}{catalogName}-{propositionCategory.SortIndex.ToString().PadLeft(3, '0') + propositionCategory.Id + categoryName.ProposeValidId()}";
            Category category = null;

            //Get or create category
            if (await _doesEntityExistPipeline.Run(new FindEntityArgument(typeof(Category), categoryId),
                context.PipelineContextOptions))
            {
                category = await _getCategoryPipeline.Run(new GetCategoryArgument(categoryId),
                    context.PipelineContextOptions);
            }
            else
            {
                var createResult = await _createCategoryPipeline.Run(
                    new CreateCategoryArgument(catalogId,
                        propositionCategory.SortIndex.ToString().PadLeft(3, '0') + propositionCategory.Id +
                        categoryName.ProposeValidId(), categoryName, ""), context.PipelineContextOptions);
                category = createResult?.Categories?.FirstOrDefault(c => c.Id == categoryId);
            }

            if (category != null)
            {
                //Localize properties
                var localizationEntityId = LocalizationEntity.GetIdBasedOnEntityId(category.Id);
                var localizationEntity = await _findEntityPipeline.Run(new FindEntityArgument(typeof(LocalizationEntity), localizationEntityId),
                    context.PipelineContextOptions) as LocalizationEntity;

                if (localizationEntity != null)
                {
                    var displayNames = propositionCategory.Values.Select(p => new Parameter(p.Language, p.Name)).ToList();
                    localizationEntity.AddOrUpdatePropertyValue("DisplayName", displayNames);

                    var descriptions = propositionCategory.CategoryContent.Values.Select(p => new Parameter(p.Language, p.Name)).ToList();
                    localizationEntity.AddOrUpdatePropertyValue("Description", descriptions);

                    await _persistEntityPipeline.Run(new PersistEntityArgument(localizationEntity),
                        context.PipelineContextOptions);
                }

                //Add identifiers
                var identifiersComponent = category.GetComponent<IdentifiersComponent>();
                if (!identifiersComponent.CustomId.Any(i => i.Key.Equals(policy.CustomIdentifierKey)))
                    identifiersComponent.CustomId.Add(
                        new Sitecore.Commerce.Plugin.Catalogs.SyncForce.Models.CustomIdentifier(
                            policy.CustomIdentifierKey, propositionCategory.Id.ToString()));
                category.SetComponent(identifiersComponent);
                category = (await _persistEntityPipeline.Run(new PersistEntityArgument(category),
                               context.PipelineContextOptions))?.Entity as Category ?? category;

                //Add custom properties
                var categoryComponent = category.GetComponent<SyncForceCategoryComponent>();
                categoryComponent.Sortorder = propositionCategory.SortIndex;
                category.SetComponent(categoryComponent);
                category = (await _persistEntityPipeline.Run(new PersistEntityArgument(category), context.PipelineContextOptions))?.Entity as Category ?? category;

                //Create sub-categories
                if (propositionCategory?.ProductPropositionCategories?.Any() ?? false)
                {
                    foreach (var subCategory in propositionCategory.ProductPropositionCategories)
                    {
                        var s = await EnsureCategory(subCategory, catalogId, catalogName, policy, context);
                        result.AddRange(s);

                        foreach (var createdSubCategory in s)
                        {
                            var persistedSubCategory =
                                (await _persistEntityPipeline.Run(new PersistEntityArgument(createdSubCategory),
                                    context.PipelineContextOptions))?.Entity;
                            var associateResult = await _associateCategoryToParentPipeline.Run(
                                new CatalogReferenceArgument(catalogId, category.Id,
                                    persistedSubCategory?.Id ?? createdSubCategory.Id),
                                context.PipelineContextOptions);
                            category = associateResult?.Categories?.FirstOrDefault(c => c.Id == category.Id) ??
                                       category;

                        }
                    }
                }

                //Associate with catalog if top level category
                if (associateWithCatalog)
                {
                    var associateResult = await _associateCategoryToParentPipeline.Run(
                        new CatalogReferenceArgument(catalogId, catalogId, category.Id),
                        context.PipelineContextOptions);
                    category = associateResult?.Categories
                                   ?.FirstOrDefault(c => c.Id == category.Id) ?? category;
                }

                result.Add(category);
            }

            return result;
        }
    }
}
