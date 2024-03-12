namespace Dragonfly.UmbracoHelpers
{
    using Dragonfly.UmbracoModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    //using System.Web.Mvc;
    using Microsoft.AspNetCore.Html;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Web.Common;
    using Umbraco.Extensions;
    using Umbraco.Cms.Core.Models;

    /// <summary>
    /// Extension methods for Base models
    /// </summary>
    public static class ContentExtensions
    {
        #region Model Conversion for IPublishedContent & IPublishedElement - IEnumerable<T> 

        /// <summary>
        /// Convert a collection of IPublishedContent to a strongly-typed Model
        /// </summary>
        /// <typeparam name="T">Model to convert to </typeparam>
        /// <param name="Content">IPub</param>
        /// <param name="FailSilently">If true, will return any which CAN be converted
        /// (which might be less than the total passed-in),
        /// false will throw an Exception if any cannot be converted</param>
        /// <returns></returns>
        public static IList<T> ToContentModels<T>(this IEnumerable<IPublishedContent> Content, bool FailSilently = false)
        {
            if (FailSilently)
            {
                //Convert one at a time
                var list = new List<T>();
                if (Content != null)
                {
                    foreach (var item in Content)
                    {
                        try
                        {
                            var x = (T)item;
                            list.Add(x);
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }
                    }
                }
                return list;
            }
            else
            {
                //Attempt to convert them all at once - this might throw an exception if any cannot be converted
                IList<T> list = Content.Select(x => (T)x).ToList();
                return list;
            }

        }

        public static IList<T> ToContentModels<T>(this IEnumerable<IPublishedElement> Content, bool FailSilently = false)
        {
            if (FailSilently)
            {
                //Convert one at a time
                var list = new List<T>();
                if (Content != null)
                {
                    foreach (var item in Content)
                    {
                        try
                        {
                            var x = (T)item;
                            list.Add(x);
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }
                    }
                }
                return list;
            }
            else
            {
                //Attempt to convert them all at once - this might throw an exception if any cannot be converted
                IList<T> list = Content.Select(x => (T)x).ToList();
                return list;
            }
        }


        #endregion


        #region HasPropertyWithValue

        /// <summary>
        /// Checks if the model has a property and a value for the property
        /// </summary>
        /// <param name="Model">
        /// The <see cref="IPublishedContent"/> to inspect
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias on the <see cref="IPublishedContent"/>
        /// </param>
        /// <returns>
        /// A value indicating whether or not the property exists on the <see cref="IPublishedContent"/> and has a value
        /// </returns>
        public static bool HasPropertyWithValue(this IPublishedContent Model, string PropertyAlias)
        {
            return Model.HasProperty(PropertyAlias) && Model.HasValue(PropertyAlias);
        }

        ///// <summary>
        ///// Checks if the model has a property and a value for the property
        ///// </summary>
        ///// <param name="model">The <see cref="RenderModel"/></param>
        ///// <param name="propertyAlias">The Umbraco property alias on the model</param>
        ///// <returns>A value indicating whether or not the property exists on the model and has a value</returns>
        //public static bool HasPropertyWithValue(this RenderModel model, string propertyAlias)
        //{
        //    return model.Content.HasPropertyWithValue(propertyAlias);
        //}

        #endregion

        #region String Properties

        ///// <summary>
        ///// Checks if the model has a property and a value for the property and returns either the string representation
        ///// of the property or an empty string
        ///// </summary>
        ///// <param name="model">
        ///// The <see cref="RenderModel"/>
        ///// </param>
        ///// <param name="propertyAlias">
        ///// The Umbraco property alias
        ///// </param>
        ///// <returns>
        ///// The property value as a string or an empty string
        ///// </returns>
        //public static string GetSafeString(this RenderModel model, string propertyAlias)
        //{
        //    return model.Content.GetSafeString(propertyAlias);
        //}

        /// <summary>
        /// Checks if the model has a property and a value for the property and returns either the string representation
        /// of the property or an empty string
        /// </summary>
        /// <param name="Content">
        /// The <see cref="IPublishedContent"/> that should contain the property
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetSafeString(this IPublishedContent Content, string PropertyAlias)
        {
            return Content.GetSafeString(PropertyAlias, string.Empty);
        }

        /// <summary>
        /// Checks if the model has a property and a value for the property and returns either the string representation
        /// of the property or the default value
        /// </summary>
        /// <param name="Content">
        /// The <see cref="IPublishedContent"/> that should contain the property
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias.
        /// </param>
        /// <param name="DefaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetSafeString(this IPublishedContent Content, string PropertyAlias, string DefaultValue)
        {
            return Content.HasPropertyWithValue(PropertyAlias) ? Content.Value<string>(PropertyAlias) : DefaultValue;
        }

        #endregion

        #region Date Properties

        /// <summary>
        /// Gets a safe date time from content
        /// </summary>
        /// <param name="Content">
        /// The content.
        /// </param>
        /// <param name="PropertyAlias">
        /// The property alias.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime GetSafeDateTime(this IPublishedContent Content, string PropertyAlias)
        {
            return Content.GetSafeDateTime(PropertyAlias, DateTime.MinValue);
        }

        /// <summary>
        /// Gets a safe date time from content
        /// </summary>
        /// <param name="Content">
        /// The content.
        /// </param>
        /// <param name="PropertyAlias">
        /// The property alias.
        /// </param>
        /// <param name="DefaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime GetSafeDateTime(this IPublishedContent Content, string PropertyAlias, DateTime DefaultValue)
        {
            if (!Content.HasPropertyWithValue(PropertyAlias)) return DefaultValue;

            DateTime dt;

            return DateTime.TryParse(Content.Value<string>(PropertyAlias), out dt) ? dt : DefaultValue;
        }

        ///// <summary>
        ///// Gets a safe date time from content.
        ///// </summary>
        ///// <param name="model">
        ///// The <see cref="RenderModel"/>.
        ///// </param>
        ///// <param name="propertyAlias">
        ///// The property alias.
        ///// </param>
        ///// <returns>
        ///// The <see cref="DateTime"/>.
        ///// </returns>
        //public static DateTime GetSafeDateTime(this RenderModel model, string propertyAlias)
        //{
        //    return model.Content.GetSafeDateTime(propertyAlias);
        //}

        #endregion

        #region GUID Properties

        ///// <summary>
        ///// Checks if the model has a property and a value for the property and returns either the Guid representation
        ///// of the property or the default value
        ///// </summary>
        ///// <param name="model">
        ///// The <see cref="RenderModel"/>
        ///// </param>
        ///// <param name="propertyAlias">
        ///// The Umbraco property alias.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Guid"/>.
        ///// </returns>
        //public static Guid GetSafeGuid(this RenderModel model, string propertyAlias)
        //{
        //    return model.Content.GetSafeGuid(propertyAlias, Guid.Empty);
        //}

        /// <summary>
        /// Checks if the model has a property and a value for the property and returns either the Guid representation
        /// of the property or the default value
        /// </summary>
        /// <param name="Content">
        /// The <see cref="IPublishedContent"/>.
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static Guid GetSafeGuid(this IPublishedContent Content, string PropertyAlias)
        {
            return Content.GetSafeGuid(PropertyAlias, Guid.Empty);
        }

        /// <summary>
        /// Checks if the model has a property and a value for the property and returns either the Guid representation
        /// of the property or the default value
        /// </summary>
        /// <param name="Content">
        /// The <see cref="IPublishedContent"/>.
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias.
        /// </param>
        /// <param name="DefaultValue">
        /// The default Value.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static Guid GetSafeGuid(this IPublishedContent Content, string PropertyAlias, Guid DefaultValue)
        {
            return Content.HasPropertyWithValue(PropertyAlias)
                ? new Guid(Content.Value<string>(PropertyAlias))
                : DefaultValue;
        }

        #endregion

        #region Integer Properties

        ///// <summary>
        ///// Checks if the model has a property and a value for the property and returns either the string representation
        ///// of the property or the default value of 0
        ///// </summary>
        ///// <param name="model">
        ///// The <see cref="RenderModel"/>
        ///// </param>
        ///// <param name="propertyAlias">
        ///// The Umbraco property alias.
        ///// </param>
        ///// <returns>
        ///// The <see cref="int"/>.
        ///// </returns>
        //public static int GetSafeInt(this RenderModel model, string propertyAlias)
        //{
        //    return model.Content.GetSafeInt(propertyAlias);
        //}

        /// <summary>
        /// Checks if the model has a property and a value for the property and returns either the string representation
        /// of the property or the default value of 0
        /// </summary>
        /// <param name="Content">
        /// The <see cref="IPublishedContent"/>
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetSafeInt(this IPublishedContent Content, string PropertyAlias)
        {
            return Content.GetSafeInt(PropertyAlias, 0);
        }

        /// <summary>
        /// Checks if the model has a property and a value for the property and returns either the string representation
        /// of the property or the default value of 0
        /// </summary>
        /// <param name="Content">
        /// The <see cref="IPublishedContent"/>
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias.
        /// </param>
        /// <param name="DefaultValue">
        /// The default Value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetSafeInt(this IPublishedContent Content, string PropertyAlias, int DefaultValue)
        {
            var willWork = Content.HasPropertyWithValue(PropertyAlias);
            var propVal = Content.Value<int>(PropertyAlias);
            return willWork ? propVal : DefaultValue;
        }

        #endregion

        #region Boolean Properties

        ///// <summary>
        ///// Checks if the model has a property and a value for the property and returns either the string representation
        ///// of the property or the default value of false
        ///// </summary>
        ///// <param name="model">
        ///// The <see cref="RenderModel"/>
        ///// </param>
        ///// <param name="propertyAlias">
        ///// The Umbraco property alias.
        ///// </param>
        ///// <returns>
        ///// The <see cref="bool"/>.
        ///// </returns>
        //public static bool GetSafeBool(this RenderModel model, string propertyAlias)
        //{
        //    return model.Content.GetSafeBool(propertyAlias);
        //}

        /// <summary>
        /// Checks if the model has a property and a value for the property and returns either the string representation
        /// of the property or the default value of false
        /// </summary>
        /// <param name="Content">
        /// The <see cref="IPublishedContent"/>
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetSafeBool(this IPublishedContent Content, string PropertyAlias)
        {
            return Content.GetSafeBool(PropertyAlias, false);
        }

        /// <summary>
        /// Checks if the model has a property and a value for the property and returns either the string representation
        /// of the property or the default value of false
        /// </summary>
        /// <param name="Content">
        /// The <see cref="IPublishedContent"/>
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias.
        /// </param>
        /// <param name="DefaultValue">
        /// The default Value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetSafeBool(this IPublishedContent Content, string PropertyAlias, bool DefaultValue)
        {
            var value = Content.GetSafeString(PropertyAlias).ToLowerInvariant();
            if (string.IsNullOrEmpty(value)) return DefaultValue;
            return value == "yes" || value == "1" || value == "true";
        }

        #endregion

        #region HtmlString Properties

        ///// <summary>
        ///// Checks if the model has a property and a value for the property and returns either the <see cref="HtmlString"/> representation
        ///// of the property or an empty <see cref="HtmlString"/>
        ///// </summary>
        ///// <param name="model">
        ///// The <see cref="RenderModel"/>
        ///// </param>
        ///// <param name="propertyAlias">
        ///// The Umbraco property alias.
        ///// </param>
        ///// <returns>
        ///// The <see cref="HtmlString"/>.
        ///// </returns>
        //public static HtmlString GetSafeHtmlString(this RenderModel model, string propertyAlias)
        //{
        //    return model.Content.GetSafeHtmlString(propertyAlias);
        //}

        /// <summary>
        /// Checks if the model has a property and a value for the property and returns either the <see cref="HtmlString"/> representation
        /// of the property or an empty <see cref="HtmlString"/>
        /// </summary>
        /// <param name="Content">
        /// The <see cref="IPublishedContent"/> that should contain the property
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias.
        /// </param>
        /// <returns>
        /// The <see cref="HtmlString"/>.
        /// </returns>
        public static HtmlString GetSafeHtmlString(this IPublishedContent Content, string PropertyAlias)
        {
            return Content.GetSafeHtmlString(PropertyAlias, string.Empty);
        }

        /// <summary>
        /// Checks if the model has a property and a value for the property and returns either the <see cref="HtmlString"/> representation
        /// of the property or the default <see cref="HtmlString"/>
        /// </summary>
        /// <param name="Content">
        /// The <see cref="IPublishedContent"/> that should contain the property
        /// </param>
        /// <param name="PropertyAlias">
        /// The Umbraco property alias
        /// </param>
        /// <param name="DefaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="HtmlString"/>.
        /// </returns>
        public static HtmlString GetSafeHtmlString(this IPublishedContent Content, string PropertyAlias, string DefaultValue)
        {
            return Content.HasPropertyWithValue(PropertyAlias)
                       ? Content.Value<HtmlString>(PropertyAlias)
                       : new HtmlString(DefaultValue);
        }

        #endregion

        #region IPublishedContent Properties

        /// <summary>
        /// Checks if a property exists and has a value, returns the default for the type if not 
        /// </summary>
        /// <param name="Content">Node to get the property value from</param>
        /// <param name="PropertyAlias">Alias of the property</param>
        /// <param name="DefaultIfNone">Value to return if missing from Node (default is the Type default - ex: 0, false, "", null, etc.)</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetSafePropertyValue<T>(this IPublishedContent Content, string PropertyAlias, T DefaultIfNone = default(T))
        {
            if (Content.HasPropertyWithValue(PropertyAlias))
            {
                var value = Content.Value<T>(PropertyAlias);
                return value;
            }
            else
            { return DefaultIfNone; }
        }

        ///// <summary>
        ///// Gets a content Id from a content picker and renders it as <see cref="IPublishedContent"/>.
        ///// </summary>
        ///// <param name="model">
        ///// The current <see cref="RenderModel"/>.
        ///// </param>
        ///// <param name="propertyAlias">
        ///// The property alias.
        ///// </param>
        ///// <param name="umbraco">
        ///// The <see cref="UmbracoHelper"/>.
        ///// </param>
        ///// <returns>
        ///// The <see cref="IPublishedContent"/> from the content picker.
        ///// </returns>
        //public static IPublishedContent GetSafeContent(this RenderModel model, string propertyAlias, UmbracoHelper umbraco)
        //{
        //    return model.Content.GetSafeContent(propertyAlias, umbraco);
        //}

        /// <summary>
        /// Gets a content Id from a content picker and renders it as <see cref="IPublishedContent"/>.
        /// </summary>
        /// <param name="Content">
        /// The current <see cref="IPublishedContent"/>.
        /// </param>
        /// <param name="PropertyAlias">
        /// The property alias.
        /// </param>
        /// <param name="Umbraco">
        /// The <see cref="UmbracoHelper"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IPublishedContent"/> from the content picker.
        /// </returns>
        public static IPublishedContent GetSafeContent(this IPublishedContent Content, string PropertyAlias)
        {
            if (Content.HasPropertyWithValue(PropertyAlias))
            {
                var iPub = Content.Value<IPublishedContent>(PropertyAlias);
                return iPub;
            }
            else
            { return null; }
        }

        public static IEnumerable<IPublishedContent> GetSafeMultiContent(this IPublishedContent Content, string PropertyAlias)
        {
            if (Content.HasPropertyWithValue(PropertyAlias))
            {
                var iPubs = Content.Value<IEnumerable<IPublishedContent>>(PropertyAlias);
                return iPubs;
            }
            else
            { return new List<IPublishedContent>(); }
        }


        #endregion

        #region Get First Matching Property Value

        public static T GetFirstMatchingPropValue<T>(this IPublishedContent Content, IEnumerable<string> PropsToTest)
        {
            T returnVal = default(T);
            bool flag = true;
            foreach (string propertyAlias in PropsToTest)
            {
                if (flag)
                {
                    var safeVal = Content.GetSafePropertyValue<T>(propertyAlias);
                    var hasNoValue = EqualityComparer<T>.Default.Equals(safeVal, returnVal);
                    if (!hasNoValue)
                    {
                        returnVal = safeVal;
                        flag = false;
                    }
                }
            }
            return returnVal;
        }

        public static string GetFirstMatchingPropValueString(this IPublishedContent Content, IEnumerable<string> PropsToTest)
        {
            string str = "";
            bool flag = true;
            foreach (string propertyAlias in PropsToTest)
            {
                if (flag)
                {
                    string safeString = Content.GetSafeString(propertyAlias, "");
                    if (safeString != "")
                    {
                        str = safeString;
                        flag = false;
                    }
                }
            }
            return str;
        }

        public static DateTime GetFirstMatchingPropValueDate(this IPublishedContent Content, IEnumerable<string> PropsToTest)
        {
            var defaultVal = DateTime.MinValue;
            DateTime returnVal = defaultVal;
            bool flag = true;
            foreach (string propertyAlias in PropsToTest)
            {
                if (flag)
                {
                    var safeVal = Content.GetSafeDateTime(propertyAlias, defaultVal);
                    if (safeVal != defaultVal)
                    {
                        returnVal = safeVal;
                        flag = false;
                    }
                }
            }
            return returnVal;
        }

        public static Int32 GetFirstMatchingPropValueInt(this IPublishedContent Content, IEnumerable<string> PropsToTest)
        {
            var defaultVal = 0;
            Int32 returnVal = defaultVal;
            bool flag = true;
            foreach (string propertyAlias in PropsToTest)
            {
                if (flag)
                {
                    var safeVal = Content.GetSafeInt(propertyAlias, defaultVal);
                    if (safeVal != defaultVal)
                    {
                        returnVal = safeVal;
                        flag = false;
                    }
                }
            }
            return returnVal;
        }

        public static bool GetFirstMatchingPropValueBool(this IPublishedContent Content, IEnumerable<string> PropsToTest)
        {
            var defaultVal = false;
            bool returnVal = defaultVal;
            bool flag = true;
            foreach (string propertyAlias in PropsToTest)
            {
                if (flag)
                {
                    var safeVal = Content.GetSafeBool(propertyAlias, defaultVal);
                    if (safeVal != defaultVal)
                    {
                        returnVal = safeVal;
                        flag = false;
                    }
                }
            }
            return returnVal;
        }

        public static IPublishedContent GetFirstMatchingPropValueContent(this IPublishedContent Content, IEnumerable<string> PropsToTest)
        {
            var defaultVal = 0;
            IPublishedContent returnVal = null;
            bool flag = true;
            foreach (string propertyAlias in PropsToTest)
            {
                if (flag)
                {
                    var safeVal = Content.GetSafeContent(propertyAlias);
                    if (safeVal != null)
                    {
                        returnVal = safeVal;
                        flag = false;
                    }
                }
            }
            return returnVal;
        }

        #endregion

        #region Converting ContentNode to other Models

        /// <summary>
        /// Returns a Link object for the current IPublishedContent
        /// </summary>
        /// <param name="CurrentPage"></param>
        /// <returns></returns>
        public static Link ToLink(this IPublishedContent CurrentPage)
        {
            Link returnLink = new Link();
            returnLink.Udi = CurrentPage.ToUdi();
            returnLink.Url = CurrentPage.Url();
            returnLink.Name = CurrentPage.Name;
            returnLink.Target = "_self";

            if (returnLink.Udi.EntityType == Constants.UdiEntityType.Document)
            {
                returnLink.Type = LinkType.Content;

            }
            else if (returnLink.Udi.EntityType == Constants.UdiEntityType.Media)
            {
                returnLink.Type = LinkType.Media;
            }

            return returnLink;
        }

        #endregion

    }
}