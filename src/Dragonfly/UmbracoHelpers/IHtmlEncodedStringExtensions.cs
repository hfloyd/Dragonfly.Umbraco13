namespace Dragonfly.UmbracoHelpers
{
	using Dragonfly.NetHelpers;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Microsoft.AspNetCore.Html;
	using Umbraco.Cms.Core.Strings;

	/// <summary>
	/// Extension Methods for Umbraco-specific Type 'IHtmlEncodedString'
	/// </summary>
	public static class IHtmlEncodedStringExtensions
	{
		#region Testing HtmlString Values

		/// <summary>
		/// Checks for content 
		/// </summary>
		/// <param name="HtmlString">String to test</param>
		/// <param name="EmptyParagraphsIsNull">Should a string made up of only empty &lt;p&gt; tags be considered null?</param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this IHtmlEncodedString? HtmlString, bool EmptyParagraphsIsNull=true)
		{
			if (HtmlString == null)
				return true;

			var html = new HtmlString(HtmlString.ToHtmlString());
			return Dragonfly.NetHelpers.Html.IsNullOrEmpty(html, EmptyParagraphsIsNull);

		}
		#endregion

		#region Altering HtmlString Values

		/// <summary>
		/// Takes a List of IHtmlEncodedStrings and concatenates them together using a provided delimiter
		/// </summary>
		/// <param name="List">List of IHtmlEncodedStrings</param>
		/// <param name="Delimiter">string to separate HTML Strings</param>
		/// <returns>IHtmlString</returns>
		public static HtmlString ToConcatenatedHtmlString(this List<IHtmlEncodedString> List, string Delimiter)
		{
			var htmlList = new List<HtmlString>();
			foreach (var s in List)
			{
				htmlList.Add(new HtmlString(s.ToHtmlString()));
			}
			return Dragonfly.NetHelpers.Html.ToConcatenatedHtmlString(htmlList, Delimiter);

		}


		/// <summary>
		/// Remove all &lt;p&gt; tags
		/// </summary>
		/// <param name="Html"></param>
		/// <param name="RetainBreaks">Replaces the paragraph tag with two &lt;br&gt; tags</param>
		/// <returns></returns>
		public static HtmlString RemoveAllParagraphTags(this IHtmlEncodedString? Html, bool RetainBreaks)
		{
			var html =Html!=null ? new HtmlString(Html.ToHtmlString()):new HtmlString("");
			return Dragonfly.NetHelpers.Html.RemoveAllParagraphTags(html, RetainBreaks);
		}


		/// <summary>
		/// Removes surrounding &lt;p&gt; tags
		/// </summary>
		/// <param name="HtmlToFix"></param>
		/// <returns></returns>
		public static HtmlString RemoveOuterParagraphTags(this IHtmlEncodedString? HtmlToFix)
		{
			var html =HtmlToFix!=null ? new HtmlString(HtmlToFix.ToHtmlString()):new HtmlString("");
			return Dragonfly.NetHelpers.Html.RemoveOuterParagrahTags(html);
		}

		/// <summary>
		/// Removes all Tags from IHtmlEncodedString
		/// </summary>
		/// <param name="Input">Original IHtmlEncodedString</param>
		/// <returns></returns>
		public static string StripHtml(this IHtmlEncodedString? Input)
		{
			var html =Input!=null? new HtmlString(Input.ToHtmlString()): new HtmlString("");
			return Dragonfly.NetHelpers.Html.StripHtml(html);
		}

		/// <summary>
		/// Truncates a string to a given length, can add a ellipsis at the end (...).
		/// Method checks for open HTML tags, and makes sure to close them.
		/// </summary>
		public static HtmlString Truncate(this IHtmlEncodedString? Text, int Length, bool AddEllipsis, bool TreatTagsAsContent)
		{
			var html =Text!=null? new HtmlString(Text.ToHtmlString()) : new HtmlString("");
			return Dragonfly.NetHelpers.Html.Truncate(html, Length, AddEllipsis, TreatTagsAsContent);
		}

		#endregion

		#region If

		/// <summary>
		/// If the test is true, the string valueIfTrue will be returned, otherwise the valueIfFalse will be returned.
		/// </summary>
		public static HtmlString If(bool Test, IHtmlEncodedString ValueIfTrue, IHtmlEncodedString ValueIfFalse)
		{
			var htmlTrue = new HtmlString(ValueIfTrue.ToHtmlString());
			var htmlFalse = new HtmlString(ValueIfFalse.ToHtmlString());
			return Dragonfly.NetHelpers.Html.If(Test, htmlTrue, htmlFalse);
		}

		/// <summary>
		/// If the test is true, the string valueIfTrue will be returned, otherwise an empty string will be returned.
		/// </summary>
		public static HtmlString If(bool Test, IHtmlEncodedString ValueIfTrue)
		{
			var htmlTrue = new HtmlString(ValueIfTrue.ToHtmlString());
			return Dragonfly.NetHelpers.Html.If(Test, htmlTrue);
		}
		#endregion

		/// <summary>Removes all &lt;p&gt; tags from HTML</summary>
		/// <param name="OriginalHtml"></param>
		/// <param name="ReplaceWithBr">optional - if there are multiple paragraphs, will put a &lt;br/&gt; tag between them</param>
		/// <returns></returns>
		public static HtmlString StripParagraphTags(this IHtmlEncodedString? OriginalHtml, bool ReplaceWithBr = true)
		{
			if (OriginalHtml != null)
			{
				string str = !string.IsNullOrEmpty(OriginalHtml.ToString())
					? OriginalHtml.ToString().StripParagraphTags(ReplaceWithBr)
					: "";
				return new HtmlString(str);
			}
			else
			{
				return new HtmlString("");
			}
		}
	}

}
