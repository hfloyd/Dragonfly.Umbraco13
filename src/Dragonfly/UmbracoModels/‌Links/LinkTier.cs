namespace Dragonfly.UmbracoModels
{
    using System.Collections.Generic;
    using Umbraco.Cms.Core.Models;

    /// <summary>
    /// Defines a link tier
    /// </summary>
    public interface ILinkTier 
    {
        /// <summary>
        /// Gets or sets the children of the current tier
        /// </summary>
        List<ILinkTier> Children { get; set; }

        /// <summary>
        /// Link Information (Url, Name, etc.)
        /// </summary>
        Umbraco.Cms.Core.Models.Link UmbracoLink { get; set; }

        public string LinkTitle { get; set; }
    }

    /// <summary>
    /// Represents a link tier
    /// </summary>
    public class LinkTier :  ILinkTier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkTier"/> class.
        /// </summary>
        public LinkTier()
        {
            this.Children = new List<ILinkTier>();
            this.UmbracoLink = new Link();
        }

        public LinkTier(Umbraco.Cms.Core.Models.Link Link)
        {
            this.Children = new List<ILinkTier>();
            this.UmbracoLink = Link;
        }


        #region Implementation of ILinkTier

        public List<ILinkTier> Children { get; set; }
        public Link UmbracoLink { get; set; }
        public string LinkTitle { get; set; }

        #endregion
    }
}
