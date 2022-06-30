<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server">
    void Page_Load()
    {
        Sitecore.Diagnostics.Log.Info("Warmup URL apptest2.aspx loaded", this);

        System.Threading.Thread.Sleep(10000); 
    }
</script>
App Test OK