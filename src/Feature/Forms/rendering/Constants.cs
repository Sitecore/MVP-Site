using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.Forms
{
    internal class Constants
    {
        internal class SSCAPIs
        {
            internal const string AuthenticationApi = "/sitecore/api/ssc/auth/login";
            internal const string ItemApi = "/sitecore/api/ssc/item/";
        }

        internal class ItemsIds
        {
            internal const string Categories = "{E2F6EA22-562B-4D70-A444-F5BBC81C183E}";

            internal class ApplicationSteps
            {
                internal const string Category = "{6315DCC7-5F9F-44D8-B874-AC9957B220AF}";
                internal const string PersonalInformation = "{ED7DEC1D-9C2E-4AC1-8AEF-709CB0AB3506}";
                internal const string Objectives = "{DCC9E1C8-2D79-4829-8862-8013D647BF48}";
                internal const string Socials = "{49E75B4C-3398-48C8-BFF3-5EF91BE15025}";
                internal const string Contributions = "{7097BB71-8E97-401A-BFB3-14D5FFA5DF64}";
                internal const string Confirmation = "{6B57C63E-5B8F-4C9C-9E71-DDA4449F8B2D}";
            }
        }

        internal class SessionConstants
        {
            internal const string UserApplicationId = "UserApplicationId";
        }
    }
}
