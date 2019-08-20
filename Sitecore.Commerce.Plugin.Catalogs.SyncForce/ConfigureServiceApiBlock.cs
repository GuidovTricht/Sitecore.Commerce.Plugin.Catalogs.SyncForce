using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Commands;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce
{
    /// <summary>
    /// Defines a block which configures the OData model
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("SamplePluginConfigureServiceApiBlock")]
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="modelBuilder">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ODataConventionModelBuilder"/>.
        /// </returns>
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument cannot be null.");

            // Add the entities
            modelBuilder.AddEntityType(typeof(SynchronizeCatalogCommand));

            // Add the entity sets

            // Add complex types

            // Add unbound functions

            // Add unbound actions
            var synchronizeCatalog = modelBuilder.Action("SynchronizeCatalog");
            synchronizeCatalog.Returns<string>();
            synchronizeCatalog.Parameter<string>("catalogId");
            synchronizeCatalog.ReturnsFromEntitySet<CommerceCommand>("Commands");

            var synchronizeProduct = modelBuilder.Action("SynchronizeProduct");
            synchronizeProduct.Returns<string>();
            synchronizeProduct.Parameter<string>("productId");
            synchronizeProduct.Parameter<string>("externalProductId");
            synchronizeProduct.Parameter<string>("entityId");
            synchronizeProduct.ReturnsFromEntitySet<CommerceCommand>("Commands");

            return Task.FromResult(modelBuilder);
        }
    }
}
