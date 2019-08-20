using System;
using System.Threading.Tasks;
using System.Web.Http.OData;
using Microsoft.AspNetCore.Mvc;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Commands;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Defines a controller
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.CommerceController" />
    public class CommandsController : CommerceController
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Catalogs.SyncForce.Controllers.CommandsController" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }

        /// <summary>
        /// Samples the command.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="IActionResult"/></returns>
        [HttpPost]
        [Route("SynchronizeCatalog()")]
        public async Task<IActionResult> SynchronizeCatalog([FromBody] ODataActionParameters value)
        {
            if (!ModelState.IsValid || value == null)
            {
                return new BadRequestObjectResult(ModelState);
            }
            if (!value.ContainsKey("catalogId"))
            {
                return new BadRequestObjectResult(value);
            }

            var catalogId = value["catalogId"].ToString();

            if (string.IsNullOrWhiteSpace(catalogId))
            {
                return new BadRequestObjectResult(value);
            }

            var command = Command<SynchronizeCatalogCommand>();
            await command.Process(CurrentContext, catalogId);
            return new ObjectResult(command);
        }

        [HttpPost]
        [Route("SynchronizeProduct()")]
        public async Task<IActionResult> SynchronizeProduct([FromBody] ODataActionParameters value)
        {
            if (!ModelState.IsValid || value == null)
            {
                return new BadRequestObjectResult(ModelState);
            }
            if (!value.ContainsKey("productId") && !value.ContainsKey("entityId") && !value.ContainsKey("externalProductId") && !value.ContainsKey("catalogId"))
            {
                return new BadRequestObjectResult(value);
            }

            var productId = value["productId"].ToString();
            var catalogId = value["catalogId"].ToString();
            var entityId = value["entityId"].ToString();
            var externalProductId = Convert.ToInt32(value["externalProductId"]);

            if (string.IsNullOrWhiteSpace(productId) && string.IsNullOrWhiteSpace(entityId) && externalProductId > 0)
            {
                return new BadRequestObjectResult(value);
            }

            var command = Command<SynchronizeProductCommand>();
            await command.Process(CurrentContext, catalogId, productId, entityId, externalProductId);
            return new ObjectResult(command);
        }
    }
}