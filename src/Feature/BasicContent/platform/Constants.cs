using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.BasicContent
{
    public class Constants
    {
        public class Templates
        {
            public static readonly ID ContentList = new ID("{6193DCFC-B632-4FA5-9411-F9588470C1C9}");
            public static readonly ID ContentListItem = new ID("{AB41E387-F300-470C-88D1-6FCD5E12C5D8}");
        }

        public class FieldNames
        {
            public const string PanelTitle = "ContentListTitle";

            public const string Title = "ItemTitle";
            public const string Subtitle = "ItemSubtitle";
            public const string Text = "ItemText";
            public const string Link = "ItemLink";
        }

        public class RenderingParameters
        {
            public const string ListType = "ContentListType";
            public const string Limit = "ContentListLimit";
        }
    }
}