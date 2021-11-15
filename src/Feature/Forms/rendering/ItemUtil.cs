using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mvp.Feature.Forms
{
	public static class ItemUtil
	{
        private static int _maxItemNameLength = 100;
        private static string _invalidItemNameChars = "\\/:?\"<>|[]()";
        private static string _itemNameValidation = @"^[\w\*\$][\w\s\-\$]*(\(\d{1,}\)){0,1}$";
        public static string ProposeValidItemName(string name)
        {
            if (ItemUtil.IsItemNameValid(name))
            {
                return name;
            }
            string aSCII = ItemUtil.ConvertToASCII(name.Trim());
            char[] invalidItemNameChars = _invalidItemNameChars.ToCharArray();
            for (int i = 0; i < (int)invalidItemNameChars.Length; i++)
            {
                char chr = invalidItemNameChars[i];
                aSCII = aSCII.Replace(chr.ToString(), string.Empty);
            }
            aSCII = aSCII.Trim();
            if (ItemUtil.IsItemNameValid(aSCII))
            {
                return aSCII;
            }
            StringBuilder stringBuilder = new StringBuilder();
            for (int j = 0; j < aSCII.Length; j++)
            {
                char chr1 = aSCII[j];
                if (char.IsLetterOrDigit(chr1))
                {
                    stringBuilder.Append(chr1);
                }
                else if (char.IsWhiteSpace(chr1))
                {
                    stringBuilder.Append(" ");
                }
            }
            aSCII = stringBuilder.ToString().Trim();
            if (aSCII.Length > _maxItemNameLength)
            {
                aSCII = aSCII.Remove(_maxItemNameLength).Trim();
            }
            if (ItemUtil.IsItemNameValid(aSCII))
            {
                return aSCII;
            }
            return name;
        }

        private static bool IsItemNameValid(string name)
        {
            return ItemUtil.GetItemNameError(name).Length == 0;
        }

        private static string GetItemNameError(string name)
        {
            if (name.Length == 0)
            {
                return "An item name cannot be blank.";
            }
            if (name.Length > _maxItemNameLength)
            {
                return "An item name lenght should be less or equal to {0}.";
            }
            if (name[name.Length - 1] == '.')
            {
                return "An item name cannot end in a period (.)";
            }
            if (name.Length != name.Trim().Length)
            {
                return "An item name cannot start or end with blanks.";
            }
            if (name.IndexOfAny(_invalidItemNameChars.ToCharArray()) >= 0)
            {
                return string.Format("An item name cannot contain any of the following characters: {0} (controlled by the setting InvalidItemNameChars)", _itemNameValidation);
            }
            if (_itemNameValidation.Length > 0 && !Regex.IsMatch(name, _itemNameValidation, RegexOptions.ECMAScript))
            {
                return "An item name must satisfy the pattern: {0} (controlled by the setting ItemNameValidation)";
            }
           
            return string.Empty;
        }

        private static string ConvertToASCII(string str)
        {
            return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(str));
        }
    }
}
