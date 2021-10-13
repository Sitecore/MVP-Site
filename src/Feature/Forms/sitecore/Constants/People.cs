using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Constants
{
    public struct People
    {
        public struct Folder
        {
            public static ID FOLDER_ID = ID.Parse("{64F31E3A-2040-4E69-B9A7-6830CBE669D2}");
        }
        public struct Template
        {
            public static ID TEMPLATE_ID = ID.Parse("{AD9C7837-8660-4360-BA2B-7ADDF4163685}");
        }
        public struct Fields
        {
            public static ID PEOPLE_FIRST_NAME = ID.Parse("{022DE4C6-82CF-437C-9424-2FDDF1A94F68}");
            public static ID PEOPLE_LAST_NAME = ID.Parse("{C0B5D514-19C3-430E-9A5A-42763211573D}");
            public static ID PEOPLE_EMAIL = ID.Parse("{8561A65D-741E-4EAD-985E-EA1880E196A9}");
        }
    }
}