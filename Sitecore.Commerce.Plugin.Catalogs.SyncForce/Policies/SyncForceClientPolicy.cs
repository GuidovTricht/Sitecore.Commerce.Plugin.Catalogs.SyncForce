using Sitecore.Commerce.Core;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Policies
{
    public class SyncForceClientPolicy : Policy
    {
        public string Endpoint { get; set; }
        public string SecurityCode { get; set; }
        public int? PropositionId { get; set; }
        public string DefaultLanguage { get; set; }
        public string CustomIdentifierKey { get; set; }
        public string CommonComposerTemplateName { get; set; }

        public SyncForceClientPolicy()
        {
            this.Endpoint = string.Empty;
            this.SecurityCode = string.Empty;
            this.PropositionId = 1;
            this.DefaultLanguage = "en";
            this.CustomIdentifierKey = "SyncForceId";
            this.CommonComposerTemplateName = "SyncForce common properties";
        }
    }
}
