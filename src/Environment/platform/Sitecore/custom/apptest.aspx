<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server">
    void Page_Load()
    {
        Sitecore.Diagnostics.Log.Info("Warmup URL 1 loaded", this);

        System.Threading.Thread.Sleep(60000); 
    }
</script>
App Test OK