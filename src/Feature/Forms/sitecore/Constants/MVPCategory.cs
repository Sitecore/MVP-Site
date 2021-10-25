using Sitecore.Data;

namespace Mvp.Feature.Forms.Constants
{
    public struct MVPCategory
    {
        public struct Folder
        {
            public static ID FOLDER_ID = ID.Parse("{E2F6EA22-562B-4D70-A444-F5BBC81C183E}");//this is the content folder
        }

        public struct Template
        {
            public static ID TEMPLATE_ID = ID.Parse("{CDD8427E-FA86-4549-A647-D665164F2F77}");

            public struct Fields
            {
                public static ID NAME = ID.Parse("{6E7574BD-3578-4C7D-A438-FB86D7C1C927}");
                public static ID ACTIVE = ID.Parse("{E20FDA97-3F0C-49A0-9257-D55D006A2EFE}");
            }
        }
    }
}