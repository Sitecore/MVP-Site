using Sitecore.Data;

namespace Mvp.Foundation.People
{
    public static class Constants
    {
        public static class Folders
		{
            public static readonly ID MVPRepoFolder = new ID("{D3B2AC74-F73D-450D-A040-B40D53E314EC}");

            public static readonly ID CountriesTagFolder = new ID("{A7F9B74F-3AA5-41F3-9109-28DA2E1C2480}");
            public static readonly ID AwardsTagFolder = new ID("{92F9DD5A-A8A5-46B2-97FE-A3924548DAC5}");
            public static readonly ID YearsTagFolder = new ID("{5007E4C7-A04E-47D4-A10B-E339A9CA6731}");
        }
        public static class Templates
        {
            public static readonly ID Person = new ID("{AD9C7837-8660-4360-BA2B-7ADDF4163685}");
            public static readonly ID PersonAward = new ID("{D5412132-BBC8-479F-B358-E5167E6FCF1B}");
            public static readonly ID Country = new ID("{1FC8657F-F737-47D6-A52B-67E8BFF8A390}");
            public static readonly ID YearCategory = new ID("{7007DB97-C724-464B-A735-B301844F7D82}");
            public static readonly ID Year = new ID("{F27C2D30-C390-47D2-8DC9-F73FE114FA70}");
            public static readonly ID MVPType = new ID("{CDD8427E-FA86-4549-A647-D665164F2F77}");
        }

        public static class FieldNames
        {
            public const string FirstName = "First Name";
            public const string LastName = "Last Name";
            public const string Email = "Email";
            public const string Introduction = "Introduction";
            public const string Country = "Country";
            public const string Awards = "Awards";

            public const string Type = "Type";
        }

    }
}