namespace Dragonfly.UmbracoHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Extensions;

    public static class LinksHelper
    {
        public static Link ConvertNodeToLink(IPublishedContent Node)
        {
            var isMedia = Node.ItemType == PublishedItemType.Media;

            var umbLink = new Link();
            umbLink.Name = Node.Name;
            umbLink.Target = isMedia ? "_blank" : "_self";
            umbLink.Type = isMedia ? LinkType.Media : LinkType.Content;
            umbLink.Udi = Node.ToUdi();
            umbLink.Url = Node.Url(); 

            return umbLink;
        }

        public static Link CreateExternalLink(string Url, string Title = "", bool OpenInSameWindow = false)
        {
            var umbLink = new Link();
            umbLink.Name = Title;
            umbLink.Target = OpenInSameWindow ? "_self" : "_blank";
            umbLink.Type = LinkType.External;
            umbLink.Udi = null;
            umbLink.Url = Url;

            return umbLink;
        }
    }
}
