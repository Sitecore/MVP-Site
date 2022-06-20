<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server">
    void Page_Load()
    {
        if (System.Web.HttpContext.Current.Cache.Get("APPINIT") == "1")
            {
                Sitecore.Diagnostics.Log.Info("Warmup cache exists", this);
            }
            else
            {
                Sitecore.Diagnostics.Log.Info("Warmup cache not present", this);
            }    
    }
</script>
Cache test