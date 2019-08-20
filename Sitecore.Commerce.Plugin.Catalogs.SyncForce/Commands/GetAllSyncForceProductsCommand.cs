using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Helpers;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Services;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Commands
{
    public class GetAllSyncForceProductsCommand : CommerceCommand
    {
        public GetAllSyncForceProductsCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<GetPropositionFullProductsWrapper> Process(CommerceContext commerceContext)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var syncForceClientPolicy = ConnectionHelper.GetSyncForceClientPolicy(commerceContext);
                var syncForceClient = ConnectionHelper.GetSyncForceClient(commerceContext);
                return await syncForceClient.GetPropositionFullProductsAsync(syncForceClientPolicy.SecurityCode, syncForceClientPolicy.PropositionId.Value);
            }
        }
    }
}
