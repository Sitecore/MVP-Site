using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.BasicContent.Shared.Models
{
    public class ContentList
    {
        public string Title { get; set; }
        public IEnumerable<ContentListItem> Items { get; set; }
        public string ContentListType { get; set; }
        public string ContentListLimit{ get; set; }
    }
}
