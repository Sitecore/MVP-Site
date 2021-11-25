using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Constants
{
    public struct Country
    {
        public struct Folder
        {
            public static ID FOLDER_ID = ID.Parse("{D18D6FFE-DEF4-4D7B-AEA0-13F06B9030A3}");//this is the content folder
        }
        public struct Template
        {
            public static ID TEMPLATE_ID = ID.Parse("{1FC8657F-F737-47D6-A52B-67E8BFF8A390}");
        
            public struct Fields
            {
                public static ID NAME = ID.Parse("{09821E82-2AD7-49F0-B4CD-E4ABF8D90983}");
                public static ID DESCRIPTION = ID.Parse("{89D4F494-DCBD-408C-AD2B-2B0372D73CA8}");
                
            }
        }
    }
}