namespace Dragonfly.UmbracoHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Core;

    public static class UdiHelpers
    {
        public static IEnumerable<Udi> ToUdis(this IEnumerable<string> UdiStrings)
        {
            var udis = new List<Udi>();

            foreach (var s in UdiStrings)
            {
               var uri = new Uri(s);
               Udi newUdi = Udi.Create(uri);
               udis.Add(newUdi);
                //var isUdi = Udi.TryParse(s, out newUdi);
                //if (isUdi)
                //{ udis.Add(newUdi); }
            }

            return udis;
        }


        /// <summary>
        /// Converts a list of published content to a comma-separated string of UDI values suitable for using with the content service
        /// </summary>
        /// <param name="PubsEnum">A collection of IPublishedContent</param>
        /// <param name="UdiType">UDI Type to use (document, media, etc) (use 'Umbraco.Core.Constants.UdiEntityType.' to specify)
        /// If excluded, will try to use the DocTypeAlias to determine the UDI Type</param>
        /// <returns>A CSV string of UID values eg. umb://document/56c0f0ef0ac74b58ae1cce16db1476af,umb://document/5cbac9249ffa4f5ab4f5e0db1599a75b</returns>
        public static string ToUdiCsv(this IEnumerable<IPublishedContent> PubsEnum, string UdiType = "")
        {
            var list = new List<string>();
            if (PubsEnum != null)
            {
                foreach (var publishedContent in PubsEnum)
                {
                    if (publishedContent != null)
                    {
                        var udi = ToUdi(publishedContent, UdiType);
                        list.Add(udi.ToString());
                    }
                }
            }
            return string.Join(",", list);
        }

        /// <summary>
        /// Converts an IPublishedContent to a UDI string suitable for using with the content service
        /// </summary>
        /// <param name="PublishedContent">Node to use</param>
        /// <param name="UdiType">UDI Type to use (document, media, etc) (use 'Umbraco.Core.Constants.UdiEntityType.' to specify)
        /// If excluded, will try to use the PublishedItemType to determine the UDI Type</param>
        /// <returns></returns>
        public static Udi ToUdi(this IPublishedContent PublishedContent, string UdiType = "")
        {
            if (PublishedContent != null)
            {
                var udiType = UdiType != "" ? UdiType : GetUdiType(PublishedContent);
                var udi = Udi.Create(udiType, PublishedContent.Key);
                return udi;
            }
            else
            {
                return null;
            }
        }

        ///// <summary>
        ///// Converts an IPublishedContent to a UDI string suitable for using with the content service
        ///// </summary>
        ///// <param name="PublishedContent">Node to use</param>
        ///// <param name="UdiType">UDI Type to use (document, media, etc) (use 'Umbraco.Core.Constants.UdiEntityType.' to specify)
        ///// If excluded, will try to use the DocTypeAlias to determine the UDI Type</param>
        ///// <returns></returns>
        //public static string ToUdiString(this IPublishedContent PublishedContent, string UdiType = "")
        //{
        //    if (PublishedContent != null)
        //    {
        //        var udi = PublishedContent.ToUdi(UdiType);
        //        return udi != null ? udi.ToString() : "";
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}

        /// <summary>
        /// Returns a string representation of the type for the Udi (ex: 'media' or 'document')
        /// </summary>
        /// <param name="PublishedContent">Node to get data for</param>
        /// <returns></returns>
        private static string GetUdiType(IPublishedContent PublishedContent)
        {
            var udiType = Constants.UdiEntityType.Document;

            //if it's a media or member type, use that, otherwise, document is assumed
            switch (PublishedContent.ItemType)
            {
                case PublishedItemType.Media:
                    udiType = Constants.UdiEntityType.Media;
                    break;
                case PublishedItemType.Member:
                    udiType = Constants.UdiEntityType.Member;
                    break;
            }

            return udiType;
        }

    }
}
