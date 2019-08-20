using Sitecore.Commerce.Core;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Policies
{
    public class SyncForceClientPolicy : Policy
    {
        public string Endpoint { get; set; }
        public string SecurityCode { get; set; }
        public int? PropositionId { get; set; }
        public int? FullImageMediaApplicationId { get; set; }
        public int? PdfMediaApplicationId { get; set; }
        public int? DeclarationMediaTypeId { get; set; }
        public string ProductDefinitionName { get; set; }
        public string DefaultLanguage { get; set; }
        public string CustomIdentifierKey { get; set; }
        public string TopPropositionsCategoryName { get; set; }
        public string TopMarketsCategoryName { get; set; }
        public string CommonComposerTemplateName { get; set; }

        public SyncForceClientPolicy()
        {
            this.Endpoint = string.Empty;
            this.SecurityCode = string.Empty;
            this.PropositionId = 1;
            this.FullImageMediaApplicationId = 1;
            this.PdfMediaApplicationId = 1;
            this.DeclarationMediaTypeId = 1;
            this.ProductDefinitionName = string.Empty;
            this.DefaultLanguage = "en";
            this.CustomIdentifierKey = "SyncForceId";
            this.TopPropositionsCategoryName = "Propositions";
            this.TopMarketsCategoryName = "Markets";
            this.CommonComposerTemplateName = "SyncForce common properties";
        }
    }
}
