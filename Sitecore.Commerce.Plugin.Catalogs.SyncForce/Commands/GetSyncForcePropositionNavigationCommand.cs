using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Helpers;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Services;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Commands
{
    public class GetSyncForcePropositionNavigationCommand : CommerceCommand
    {
        public GetSyncForcePropositionNavigationCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public virtual async Task<GetPropositionNavigationWrapper> Process(CommerceContext commerceContext)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var syncForceClientPolicy = ConnectionHelper.GetSyncForceClientPolicy(commerceContext);
                if (!syncForceClientPolicy.PropositionId.HasValue)
                    return null;
                var syncForceClient = ConnectionHelper.GetSyncForceClient(commerceContext);
                return await syncForceClient.GetPropositionNavigationAsync(syncForceClientPolicy.SecurityCode, syncForceClientPolicy.PropositionId.Value);
            }
        }
    }
}
