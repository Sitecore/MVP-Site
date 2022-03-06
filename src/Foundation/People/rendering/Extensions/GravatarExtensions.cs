using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace Mvp.Foundation.People.Extensions
{
    public static class GravatarExtensions
    {
        public static string ToGravatar(this string email, string ImageSize, string DefaultImageUrl)
        {
            var result = string.Format("{0}://gravatar.com/avatar/{1}?s={2}{3}",
                               "https",
                               GetMd5Hash(email.ToLowerInvariant()),
                               ImageSize,
                               "&d=" + (!string.IsNullOrEmpty(DefaultImageUrl) ? HtmlEncoder.Default.Encode(DefaultImageUrl) : DefaultImage.Default.GetDescription())
                           );

            return result;
        }

        public enum DefaultImage
        {
            /// <summary>Default Gravatar logo</summary>
            [Description("")]
            Default,

            /// <summary>404 - do not load any image if none is associated with the email hash, instead return an HTTP 404 (File Not Found) response</summary>
            [Description("404")]
            Http404,

            /// <summary>Mystery-Man - a simple, cartoon-style silhouetted outline of a person (does not vary by email hash)</summary>
            [Description("mm")]
            MysteryMan,

            /// <summary>Identicon - a geometric pattern based on an email hash</summary>
            [Description("identicon")]
            Identicon,

            /// <summary>MonsterId - a generated 'monster' with different colors, faces, etc</summary>
            [Description("monsterid")]
            MonsterId,

            /// <summary>Wavatar - generated faces with differing features and backgrounds</summary>
            [Description("wavatar")]
            Wavatar,

            /// <summary>Retro - awesome generated, 8-bit arcade-style pixelated faces</summary>
            [Description("retro")]
            Retro
        }

        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false).ToArray();

                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return en.ToString();
        }

        public static string GetMd5Hash(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}