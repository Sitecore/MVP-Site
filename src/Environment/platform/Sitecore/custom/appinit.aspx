<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server">
    void Page_Load()
    {
        if (HttpContext.Current.Request.QueryString.Count > 0)
        {
            HttpContext.Current.Cache.Remove("APPINIT");
        }
        else
        {
            HttpContext.Current.Cache.Insert("APPINIT", "1", null, DateTime.Now.AddSeconds(600), TimeSpan.Zero);
        }
    }
</script>
OK