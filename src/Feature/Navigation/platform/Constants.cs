using Sitecore.Data;

namespace Mvp.Feature.Navigation
{
    public static class Constants
    {
        public static class Templates
        {
            public static readonly ID TopLink = new ID("{E4986869-E4B1-4990-8669-52ABF69CD9CF}");
            public static readonly ID HomePage = new ID("{9C1E30A0-8691-4AD4-BEF0-E1F446EEF8F3}");
            public static readonly ID NavigationItem = new ID("{2FA21400-EB0C-4339-ADC0-71FB2D762256}");            
        }

        public static class FieldNames
        {
            public const string Link = "Link";
            public const string MenuTitle = "Menu Title";
            public const string IncludeInMenu = "Include in Menu";
        }

        public static class FieldValues
        {
            public const string CheckboxTrue = "1";
            public const string CheckboxFalse = "0";
        }
    }
}