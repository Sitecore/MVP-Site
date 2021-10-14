using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Constants
{
    public struct ApplicationStep
    {
        public struct Folder
        {
            public static ID FOLDER_ID = ID.Parse("{8BE29760-EAEC-49CA-BE48-C5C68FD887C5}");
        }
        public struct Template
        {
            public static ID TEMPLATE_ID = ID.Parse("{F1C4767E-0344-4619-9D00-62FB519C07B1}");
        }
        public struct Fields
        {
            public static ID APPLICATION_STEP_TITLE = ID.Parse("{28EFA042-B2BC-45A2-BD09-07A79CDE8467}");
            public static ID APPLICATION_STEP_VIEW_PATH = ID.Parse("{B06C3AAC-D999-4A81-BBD9-EB53D2B791A1}");
        }
    }
}