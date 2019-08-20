using System;
using System.ServiceModel;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Policies;
using Sitecore.Commerce.Plugin.Catalogs.SyncForce.Services;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Helpers
{
    public class ConnectionHelper
    {
        public static SyncForceClientPolicy GetSyncForceClientPolicy(CommerceContext context)
        {
            if (!context.HasPolicy<SyncForceClientPolicy>())
            {
                throw new Exception("GetSyncForceClientPolicy could not find a SyncForceClientPolicy to be returned for API Connection");
            }

            return context.GetPolicy<SyncForceClientPolicy>();
        }

        public static ProductServiceContractClient GetSyncForceClient(CommerceContext context)
        {
            var syncForceClientPolicy = GetSyncForceClientPolicy(context);

            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(syncForceClientPolicy.Endpoint);

            binding.MaxReceivedMessageSize = 200000000;
            binding.SendTimeout = new System.TimeSpan(0, 5, 0);
            binding.ReceiveTimeout = new System.TimeSpan(0, 5, 0);

            return new ProductServiceContractClient(binding, endpoint);
        }
    }
}
