using System;
using System.Collections.Generic;

namespace Sitecore.Commerce.Plugin.Catalogs.SyncForce.Models
{
    /// <summary>
    /// To use custom identifiers in the IdentifiersComponent we need to fix an issue where the original CustomIdentifier object doesn't have an empty constructor.
    /// To track the future status of this bug report, please use the reference number 41392.
    /// </summary>
    public class CustomIdentifier : Sitecore.Commerce.Plugin.Catalog.CustomIdentifier
    {
        public CustomIdentifier() : base(String.Empty, String.Empty)
        {
        }

        public CustomIdentifier(KeyValuePair<string, string> nameAndId) : base(nameAndId)
        {
        }

        public CustomIdentifier(string key, string value) : base(key, value)
        {
        }
    }
}
