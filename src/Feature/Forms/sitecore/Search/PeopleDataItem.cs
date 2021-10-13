using Sitecore.ContentSearch.SearchTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Search
{
    public class PeopleDataItem : SearchResultItem
    {
        [Sitecore.ContentSearch.IndexField("first_name_t_en")]
        public String FirstName { get; set; }

        [Sitecore.ContentSearch.IndexField("last_name_t_en")]
        public String LastName { get; set; }

        [Sitecore.ContentSearch.IndexField("email_t_en")]
        public String Email { get; set; }

        [Sitecore.ContentSearch.IndexField("step_sm")]
        public Guid ApplicationStep { get; set; }

        [Sitecore.ContentSearch.IndexField("application_sm")]
        public Guid Application { get; set; }
    }
}