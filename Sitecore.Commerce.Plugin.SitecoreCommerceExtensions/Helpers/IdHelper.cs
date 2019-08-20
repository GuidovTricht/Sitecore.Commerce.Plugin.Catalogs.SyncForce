using System;
using System.Security.Cryptography;
using System.Text;

namespace Sitecore.Commerce.Plugin.SitecoreCommerceExtensions.Helpers
{
    public class IdHelper
    {
        public static Guid ToUniqueId(string input)
        {
            //this code will generate a unique Guid for a string (unique with a 2^20.96 probability of a collision) 
            //http://stackoverflow.com/questions/2190890/how-can-i-generate-guid-for-a-string-values
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash);
            }
        }
    }
}
