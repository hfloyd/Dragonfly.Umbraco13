﻿namespace Dragonfly.UmbracoServices
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Dragonfly.UmbracoHelpers;
    using Dragonfly.UmbracoModels;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Web.Common;
    using Umbraco.Extensions;

    /// <summary>
    /// Service for Building various LinkTiers
    /// </summary>
    public class LinkTierBuilderService
    {
        #region Service-Level properties

        /// <summary>
        /// Property aliases to use for the Nav Display Name (in ascending order of Priority)
        /// </summary>
        public IEnumerable<string> NavDisplayNameProperties  => _navDisplayNameProperties; 

        private IEnumerable<string> _navDisplayNameProperties;
        

        /// <summary>
        /// Current page where the links are being built/rendering
        /// </summary>
        public IPublishedContent CurrentContent => _currentContent;
        private IPublishedContent _currentContent;
        #endregion

        #region Events
        /// <summary>
        /// The changed link tier event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="arg">
        /// The arg.
        /// </param>
        public delegate void ChangedLinkTierEventHandler(object sender, AddingLinkTierEventArgs arg);

        /// <summary>
        /// The adding tier.
        /// </summary>
        public event ChangedLinkTierEventHandler AddingTier;

        #endregion

        #region Ctor
        public LinkTierBuilderService(IPublishedContent CurrentContent)
        {
            _currentContent = CurrentContent;
        }

        public LinkTierBuilderService(IPublishedContent CurrentContent,IEnumerable<string> NavDisplayNamePropertyAliases)
        {
            _currentContent = CurrentContent;
            _navDisplayNameProperties = NavDisplayNamePropertyAliases;
        }

        #endregion

        #region Menu Methods

        /// <summary>
        /// Builds a <see cref="ILinkTier"/>
        /// </summary>
        /// <param name="TierContentItem">The <see cref="IPublishedContent"/> "tier" item (the parent tier)</param>
        /// <param name="CurrentContent">The CurrentContent <see cref="IPublishedContent"/> in the recursion</param>
        /// <param name="ExcludeDocumentTypes">A collection of document type aliases to exclude</param>
        /// <param name="TierLevel">The starting "tier" level. Note this is the Umbraco node level</param>
        /// <param name="MaxLevel">The max "tier" level. Note this is the Umbraco node level</param>
        /// <param name="IncludeContentWithoutTemplate">True or false indicating whether or not to include content that does not have an associated template</param>
        /// <returns>the <see cref="ILinkTier"/></returns>
        public ILinkTier BuildLinkTier(IPublishedContent TierContentItem, string[] ExcludeDocumentTypes = null, int TierLevel = 0, int MaxLevel = 0, bool IncludeContentWithoutTemplate = false)
        {
            //TODO: Fix to allow ILinkTier to LinkTier(), add param for "include doctypes"?
            var active = _currentContent.Path.Contains(TierContentItem.Id.ToString(CultureInfo.InvariantCulture));

            if (_currentContent.Level == TierContentItem.Level) active = _currentContent.Id == TierContentItem.Id;

            //Convert to Link
            var umbLink = BuilderServiceNodeToLink(TierContentItem);
          

            //Create Tier
            var tier = new LinkTier()
            {
                UmbracoLink = umbLink
                //CssClass = active ? "active" : string.Empty
            };

            if (ExcludeDocumentTypes == null) ExcludeDocumentTypes = new string[] { };

            if (TierLevel > MaxLevel && MaxLevel != 0) return tier;

            foreach (var item in TierContentItem.Children.ToList().Where(x => x.IsVisible() && (ContentHasTemplate(x) || (IncludeContentWithoutTemplate && x.IsVisible())) && !ExcludeDocumentTypes.Contains(x.ContentType.Alias)))
            {
                var newTier = this.BuildLinkTier(item, ExcludeDocumentTypes, item.Level, MaxLevel);

                if (this.AddingTier != null)
                {
                    this.AddingTier.Invoke(this, new AddingLinkTierEventArgs(tier, newTier));
                }

                tier.Children.Add(newTier);
            }

            return tier;
        }

        ///// <summary>
        ///// Constructs a breadcrumb menu  (Set the "NavDisplayNameProperties" property on the LinkHelper before calling)
        ///// </summary>
        ///// <param name="StopLevel">The "top" level at which the recursion should quit</param>
        ///// <param name="CurrentContent">The CurrentContent content</param>
        ///// <returns>List of <see cref="Link" /></returns>
        //[Obsolete("You might want to use the fully-featured version of BuildBreadCrumb()")]
        //public IEnumerable<Link> BuildBreadCrumb(int StopLevel, IPublishedContent CurrentContent)
        //{
        //    return BuildBreadCrumb(CurrentContent, "", StopLevel);
        //}

        /// <summary>
        /// Constructs a breadcrumb menu  (Set the 'NavDisplayNameProperties' property on the LinkHelper before calling)
        /// </summary>
        /// <param name="LinkTextProperty">Property to retrieve Text for the link. If the property doesn't exist or is blank, it will revert to using data in the 'NavDisplayNameProperties' property</param>
        /// <param name="MinLevel">The "top" level at which the recursion should quit (default = 1 : site root)</param>
        /// <param name="MaxLevel">The "bottom" level of parent pages which will be included in the menu (default = 0 : no limit, will include the current page)</param>
        /// <returns></returns>
        public IEnumerable<Link> BuildBreadCrumb( string LinkTextProperty = "", int MinLevel = 1, int MaxLevel = 0)
        {
            var linkText = _currentContent.Name;

            if (LinkTextProperty != "")
            {
                var linkTextPropVal = _currentContent.GetSafeString(LinkTextProperty);
                if (linkTextPropVal != "")
                {
                    linkText = linkTextPropVal;
                }
            }
            else
            {
                linkText = this.GetNavDisplayName(_currentContent);
            }

            var currentLink = BuilderServiceNodeToLink(_currentContent);
          
            //TODO - Fix this
            var links = new List<Link>();

            //if (CurrentContent.Level > MinLevel && CurrentContent.Parent != null)
            //{
            //    links.AddRange(this.BuildBreadCrumb(CurrentContent.Parent, LinkTextProperty, MinLevel, MaxLevel));
            //}

            //if (MaxLevel == 0 || CurrentContent.Level <= MaxLevel)
            //{
            //    links.Add(link);
            //}

            return links;
        }

        #endregion

        #region Additional Public Methods

       /// <summary>
        /// Uses the values in the 'NavDisplayNameProperties' property to select the best name for the nav display
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        public string GetNavDisplayName(IPublishedContent Content)
        {
            var props = this.NavDisplayNameProperties;
            var name = Content.Name; //default, if no prop values are found
            var keepLooking = true;

            foreach (var propAlias in props)
            {
                if (keepLooking)
                {
                    var propVal = Content.GetSafeString(propAlias, "");
                    if (propVal != "")
                    {
                        name = propVal;
                        keepLooking = false;
                    }
                }
            }

            return name;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Quick fix to all for checking if a content item has a template
        /// </summary>
        /// <param name="content">The <see cref="IPublishedContent"/></param>
        /// <returns>True or false indicating whether or not the content has an associated template selected</returns>
        private static bool ContentHasTemplate(IPublishedContent content)
        {
            try
            {
                var template = content.GetTemplateAlias();
                var hasTemplate = !string.IsNullOrEmpty(template);
                return hasTemplate;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Link BuilderServiceNodeToLink(IPublishedContent ContentNode)
        {
            //Convert to Link
            var umbLink = LinksHelper.ConvertNodeToLink(ContentNode);
            umbLink.Name = this.GetNavDisplayName(ContentNode);

            //TODO: Additional logic to alter link Url - maybe based on redirect props?
            //if (!ContentHasTemplate(TierContentItem))
            //{
            //    umbLink.Url = "#";
            //}

            return umbLink;
        }

        #endregion
    }

    public class AddingLinkTierEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingLinkTierEventArgs"/> class.
        /// </summary>
        /// <param name="root">
        /// The root (base link tier)
        /// </param>
        /// <param name="adding">
        /// The modified value - generally an addition or 
        /// </param>
        public AddingLinkTierEventArgs(ILinkTier root, ILinkTier adding)
        {
            Root = root;
            Adding = adding;
        }

        /// <summary>
        /// Gets or sets the root link tier.
        /// </summary>
        public ILinkTier Root { get; set; }

        /// <summary>
        /// Gets or sets the adding.
        /// </summary>
        public ILinkTier Adding { get; set; }
    }

    //public static class LinkExtensions
  //  {
        //private static UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

        //#region ToILink Extension Methods

        //public static ILink ToILink(this Newtonsoft.Json.Linq.JToken token, UmbracoHelper UmbHelper)
        //{
        //    var link = new Link();

        //    if (token != null)
        //    {
        //        dynamic tokendata = token;

        //        link.Title = tokendata.name;
        //        link.Url = tokendata.url;
        //        link.Target = tokendata.target;

        //        if (tokendata.id != 0)
        //        {
        //            link.ContentId = tokendata.id;
        //            var contentNode = UmbHelper.Content(link.ContentId);
        //            if (contentNode != null)
        //            {
        //                link.ContentNode = contentNode;
        //                link.ContentTypeAlias = contentNode.ContentType.Alias;
        //            }
        //            else
        //            {
        //                //try media node
        //                var mediaNode = UmbHelper.Media(link.ContentId);
        //                if (mediaNode != null)
        //                {
        //                    link.ContentTypeAlias = mediaNode.ContentType.Alias;
        //                    if (link.Url.StartsWith("/media/"))
        //                    {
        //                        var media = mediaNode.ToIMediaFile();
        //                        link.MediaFile = media;
        //                        link.IsMedia = true;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return link;
        //}



        //#endregion



   // }
}
