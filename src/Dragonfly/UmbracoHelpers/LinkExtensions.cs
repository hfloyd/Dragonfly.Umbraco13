namespace Dragonfly.UmbracoHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Umbraco.Cms.Core.Models;
	public static class LinkExtensions
	{
		/// <summary>
		/// Returns default value if Link URL is null or missing
		/// </summary>
		/// <param name="UmbLink">The Link</param>
		/// <param name="DefaultUrl">A URL to return if missing. Default is '#'</param>
		/// <returns></returns>
		public static string SafeLinkUrl(this Link? UmbLink, string DefaultUrl = "#")
		{
			//var defaultString = "#";
			if (UmbLink != null)
			{
				return !string.IsNullOrEmpty(UmbLink.Url) ? UmbLink.Url : DefaultUrl;
			}
			else
			{
				return DefaultUrl;
			}
		}

		/// <summary>
		/// Returns default value if Link Target is null or missing
		/// </summary>
		/// <param name="UmbLink">The Link</param>
		/// <param name="DefaultTarget">A target to return if missing. Default is '_self'</param>
		/// <returns></returns>
		public static string SafeLinkTarget(this Link? UmbLink, string DefaultTarget= "_self")
		{
			//var defaultString = "_self";

			if (UmbLink != null)
			{
				return !string.IsNullOrEmpty(UmbLink.Target) ? UmbLink.Target : DefaultTarget;
			}
			else
			{
				return DefaultTarget;
			}

		}
		
		/// <summary>
		/// Returns TRUE if the LinkType is Content and there is a value for Udi
		/// </summary>
		/// <param name="UmbLink">The Link</param>
		/// <returns></returns>
		public static bool HasContentNode(this Link? UmbLink)
		{
			if (UmbLink != null && UmbLink.Udi != null)
			{
				return UmbLink.Type == LinkType.Content;
			}
			else
			{
				return false;
			}
		}
	}
}
