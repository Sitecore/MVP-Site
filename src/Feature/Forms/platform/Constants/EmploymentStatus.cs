using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Constants
{
    public struct EmploymentStatus
    {
        public struct Folder
        {
            public static ID FOLDER_ID = ID.Parse("{965852BA-C78D-4A41-8DD8-D8CAD7032D4F}");//this is the content folder
        }
        public struct Template
        {
            public static ID TEMPLATE_ID = ID.Parse("{37421902-C3C5-49E3-BF4A-A7EE73312993}");
        
            public struct Fields
            {
                public static ID NAME = ID.Parse("{7E89334B-65AF-46FF-ABDD-F15A614AA78B}");
                public static ID DESCRIPTION = ID.Parse("{F82B02B5-9027-4646-8C33-509BC0DCF565}");
            }
        }
    }
}