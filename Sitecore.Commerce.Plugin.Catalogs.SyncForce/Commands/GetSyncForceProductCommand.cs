using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Helpers;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Services;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Commands
{
    public class GetSyncForceProductCommand : CommerceCommand
    {
        public GetSyncForceProductCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public virtual async Task<GetFullProductResponse> Process(CommerceContext commerceContext, int externalProductId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var syncForceClientPolicy = ConnectionHelper.GetSyncForceClientPolicy(commerceContext);
                var syncForceClient = ConnectionHelper.GetSyncForceClient(commerceContext);
                return await syncForceClient.GetFullProductAsync(syncForceClientPolicy.SecurityCode, externalProductId, syncForceClientPolicy.PropositionId);
            }
        }
    }
}
