<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server">
    void Page_Load()
    {
        if (HttpContext.Current.Request.QueryString.Count > 0)
        {
            HttpContext.Current.Cache.Insert("APPINIT", "1", null, DateTime.Now.AddSeconds(600), TimeSpan.Zero);
            Sitecore.Diagnostics.Log.Info("Warmup adding cache value", this);
        }
        else
        {
            HttpContext.Current.Cache.Remove("APPINIT");
            Sitecore.Diagnostics.Log.Info("Warmup removing cache value", this);
        }
    }
</script>
OK