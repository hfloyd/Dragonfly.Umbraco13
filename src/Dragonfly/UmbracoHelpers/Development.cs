namespace Dragonfly.UmbracoHelpers
{
	using HtmlAgilityPack;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using Microsoft.AspNetCore.Html;
	using Umbraco.Cms.Core;
	using Umbraco.Cms.Core.Models;
	using Umbraco.Cms.Core.Models.PublishedContent;
	using Umbraco.Cms.Core.Services;
	using Umbraco.Cms.Web.Common;
	using Umbraco.Extensions;

	/// <summary>
	/// Helpers related to Templates, Node Paths, Udis, and Getting Site Pages.
	/// Also includes functions for manipulating HTML strings.
	/// </summary>
	public static class Development
	{
		private const string ThisClassName = "Dragonfly.UmbracoHelpers.Development";

		//private static IFileService _umbFileService = FileService;
		//private static IContentService _umbContentService = ContentService;
		//private static IMediaService _umbMediaService = MediaService;

		/// <summary>
		/// Get the Alias of a template from its ID. If the Id is null or zero, "NONE" will be returned.
		/// </summary>
		/// <param name="TemplateId">Integer Id of the Template </param>
		/// <param name="UmbFileService"> Umbraco File Service</param>
		/// <returns> Alias of the template, or "NONE" if missing </returns>
		public static string GetTemplateAlias(int? TemplateId, IFileService UmbFileService)
		{
			//var umbFileService = ApplicationContext.Current.Services.FileService;
			string templateAlias;

			if (TemplateId == 0 | TemplateId == null)
			{
				templateAlias = "NONE";
			}
			else
			{
				var lookupTemplate = UmbFileService.GetTemplate(Convert.ToInt32(TemplateId));
				templateAlias = lookupTemplate.Alias;
			}

			return templateAlias;
		}

		/// <summary>
		/// Return the first descendant page in the site by its DocType
		/// </summary>
		/// <param name="UmbHelper">UmbracoHelper</param>
		/// <param name="SiteRootNodeId">ex: model.Site.Id</param>
		/// <param name="DoctypeAlias">Name of the Doctype to serach for</param>
		/// <returns>An IPublishedContent of the node, or NULL if not found. You can then cast to a strongly-typed model for the DocType (ex: new ContactUsPage(contactPage))</returns>
		public static IPublishedContent GetSitePage(UmbracoHelper UmbHelper, int SiteRootNodeId, string DoctypeAlias)
		{
			var pageMatches = GetSitePages(UmbHelper, SiteRootNodeId, DoctypeAlias).ToList();
			if (pageMatches.Any())
			{
				return pageMatches.First();
			}

			//If we get here, node not found
			return null;
		}

		/// <summary>
		/// Returns all descendant pages in the site of a specified DocType
		/// </summary>
		/// <param name="UmbHelper">UmbracoHelper</param>
		/// <param name="SiteRootNodeId">ex: model.Site.Id</param>
		/// <param name="DoctypeAlias">Name of the Doctype to serach for</param>
		/// <returns>An IEnumerable&lt;IPublishedContent&gt; of the nodes, or empty list if not found.</returns>
		public static IEnumerable<IPublishedContent> GetSitePages(UmbracoHelper UmbHelper, int SiteRootNodeId, string DoctypeAlias)
		{
			var site = UmbHelper.Content(SiteRootNodeId);
			if (site != null)
			{
				return GetSitePages(site, DoctypeAlias);
			}

			//If we get here, nodes not found
			return new List<IPublishedContent>();
		}

		/// <summary>
		/// Return the first descendant page in the site by its DocType
		/// </summary>
		/// <param name="SiteRootNode">ex: model.Site.Id</param>
		/// <param name="DoctypeAlias">Name of the Doctype to serach for</param>
		/// <returns>An IPublishedContent of the node, or NULL if not found. You can then cast to a strongly-typed model for the DocType (ex: new ContactUsPage(contactPage))</returns>
		public static IPublishedContent GetSitePage(IPublishedContent SiteRootNode, string DoctypeAlias)
		{
			var pageMatches = GetSitePages(SiteRootNode, DoctypeAlias).ToList();
			if (pageMatches.Any())
			{
				return pageMatches.First();
			}

			//If we get here, node not found
			return null;
		}

		/// <summary>
		/// Returns all descendant pages in the site of a specified DocType
		/// </summary>
		/// <param name="SiteRootNode">ex: model.Site</param>
		/// <param name="DoctypeAlias">Name of the Doctype to serach for</param>
		/// <returns>An IEnumerable&lt;IPublishedContent&gt; of the nodes, or empty list if not found.</returns>
		public static IEnumerable<IPublishedContent> GetSitePages(IPublishedContent SiteRootNode, string DoctypeAlias)
		{
			var site = SiteRootNode;
			if (site != null)
			{
				var pageMatches = site.Descendants().Where(n => n.ContentType.Alias == DoctypeAlias).ToList();
				if (pageMatches.Any())
				{
					return pageMatches;
				}
			}

			//If we get here, nodes not found
			return new List<IPublishedContent>();
		}


		///// <summary>
		///// Return a list of Prevalues for a given DataType by Name
		///// </summary>
		///// <param name="DataTypeName"></param>
		///// <returns></returns>
		//public static IEnumerable<PreValue> GetPrevaluesForDataType(string DataTypeName)
		//{
		//    IEnumerable<PreValue> toReturn = new List<PreValue>();

		//    IDataTypeDefinition dataType = Current.Services.DataTypeService.GetDataTypeDefinitionByName(DataTypeName);

		//    if (dataType == null)
		//    {
		//        return toReturn;
		//    }

		//    PreValueCollection preValues = Current.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dataType.Id);

		//    if (preValues == null)
		//    {
		//        return toReturn;
		//    }

		//    IDictionary<string, PreValue> tempDictionary = preValues.FormatAsDictionary();

		//    toReturn = tempDictionary.Select(N => N.Value);

		//    return toReturn;
		//}

		#region Node Paths

		/// <summary>
		/// Return a string representation of the path to the Node
		/// </summary>
		/// <param name="UmbContentNode">Node to Get a Path for</param>
		/// <param name="Separator">String to separate parts of the path</param>
		/// <returns></returns>
		public static string NodePath(IPublishedContent UmbContentNode, IContentService UmbContentService, IMediaService UmbMediaService, out string ErrorMessage, string Separator = " » ")
		{
			var functionName = $"{ThisClassName}.NodePath";
			ErrorMessage = "";
			string nodePathString = String.Empty;

			try
			{
				string pathIdsCsv = UmbContentNode.Path;
				if (UmbContentNode.ItemType == PublishedItemType.Content)
				{
					nodePathString = ContentNodePathFromPathIdsCsv(pathIdsCsv, UmbContentService, Separator);
				}
				else if (UmbContentNode.ItemType == PublishedItemType.Media)
				{
					nodePathString = MediaNodePathFromPathIdsCsv(pathIdsCsv, UmbMediaService, Separator);
				}
				else
				{
					var errMsg = $"Node is of type '{UmbContentNode.ItemType}' which doesn't have a path.";
					ErrorMessage = $"ERROR in {functionName} for node #{UmbContentNode.Id} ({UmbContentNode.Name}): ERROR={errMsg}.";

					var returnMsg = $"Unable to generate node path. (ERROR:{errMsg})";
					return returnMsg;
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = $"ERROR in {functionName} for node #{UmbContentNode.Id} ({UmbContentNode.Name}): ERROR={ex.Message}.";

				var returnMsg = $"Unable to generate node path. (ERROR:{ex.Message})";
				return returnMsg;
			}

			return nodePathString;
		}

		/// <summary>
		/// Return a string representation of the path to the Node
		/// </summary>
		/// <param name="UmbContentNode">Node to Get a Path for</param>
		/// <param name="Separator">String to separate parts of the path</param>
		/// <returns></returns>
		public static string NodePath(IContent UmbContentNode, IContentService UmbContentService, out string ErrorMessage, string Separator = " » ")
		{
			ErrorMessage = "";
			string nodePathString = String.Empty;

			try
			{
				string pathIdsCsv = UmbContentNode.Path;
				nodePathString = ContentNodePathFromPathIdsCsv(pathIdsCsv, UmbContentService, Separator);
			}
			catch (Exception ex)
			{
				var functionName = $"{ThisClassName}.NodePath";
				ErrorMessage = $"ERROR in {functionName} for node #{UmbContentNode.Id} ({UmbContentNode.Name}).";

				var returnMsg = $"Unable to generate node path. (ERROR:{ex.Message})";
				return returnMsg;
			}

			return nodePathString;
		}

		/// <summary>
		/// Return a string representation of the path to the Node
		/// </summary>
		/// <param name="PathIdsCsv">Comma-separated list of NodeIds representing the Path</param>
		/// <param name="Separator">String to separate parts of the path</param>
		/// <returns></returns>
		private static string ContentNodePathFromPathIdsCsv(string PathIdsCsv, IContentService UmbContentService, string Separator = " » ")
		{
			List<string> nodePathNames = new List<string>();

			string[] pathIdsArray = PathIdsCsv.Split(',');

			foreach (var sId in pathIdsArray)
			{
				if (sId != "-1")
				{
					IContent getNode = UmbContentService.GetById(Convert.ToInt32(sId));
					nodePathNames.Add(getNode.Name);
					//nodePathString = String.Concat(nodePathString, Separator, nodeName);
				}
			}

			return string.Join(Separator, nodePathNames);

		}

		/// <summary>
		/// Return a string representation of the path to the Node
		/// </summary>
		/// <param name="UmbMediaNode">Node to Get a Path for</param>
		/// <param name="Separator">String to separate parts of the path</param>
		/// <returns></returns>
		public static string MediaPath(IPublishedContent UmbMediaNode, IMediaService UmbMediaService, out string ErrorMessage, string Separator = " » ")
		{
			ErrorMessage = "";
			string nodePathString = String.Empty;

			try
			{
				string pathIdsCsv = UmbMediaNode.Path;
				nodePathString = MediaNodePathFromPathIdsCsv(pathIdsCsv, UmbMediaService, Separator);
			}
			catch (Exception ex)
			{
				var functionName = $"{ThisClassName}.MediaPath";
				ErrorMessage = $"ERROR in {functionName} for node #{UmbMediaNode.Id} ({UmbMediaNode.Name}).";

				var returnMsg = $"Unable to generate node path. (ERROR:{ex.Message})";
				return returnMsg;
			}

			return nodePathString;
		}

		/// <summary>
		/// Return a string representation of the path to the Node
		/// </summary>
		/// <param name="PathIdsCsv">Comma-separated list of NodeIds representing the Path</param>
		/// <param name="UmbMediaService"></param>
		/// <param name="Separator">String to separate parts of the path</param>
		/// <returns></returns>
		private static string MediaNodePathFromPathIdsCsv(string PathIdsCsv, IMediaService UmbMediaService, string Separator = " » ")
		{
			List<string> nodePathNames = new List<string>();

			string[] pathIdsArray = PathIdsCsv.Split(',');

			foreach (var sId in pathIdsArray)
			{
				if (sId != "-1")
				{
					IMedia getNode = UmbMediaService.GetById(Convert.ToInt32(sId));
					nodePathNames.Add(getNode.Name);
				}
			}

			return string.Join(Separator, nodePathNames);
		}

		#endregion



		#region Html

		/// <summary>
		/// Validates string as html
		/// </summary>
		/// <param name="OriginalHtml"></param>
		/// <returns>True if valid HTML, False if Invalid</returns>
		public static bool HtmlIsValid(this string OriginalHtml)
		{
			IEnumerable<HtmlParseError> validationErrors;
			return HtmlIsValid(OriginalHtml, out validationErrors);
		}

		/// <summary>
		/// Validates string as html, returns errors
		/// </summary>
		/// <param name="OriginalHtml"></param>
		/// <param name="ValidationErrors">Variable of type IEnumerable&lt;HtmlParseError&gt;</param>
		/// <returns></returns>
		public static bool HtmlIsValid(this string OriginalHtml, out IEnumerable<HtmlParseError> ValidationErrors)
		{
			if (!OriginalHtml.IsNullOrWhiteSpace())
			{
				HtmlDocument doc = new HtmlDocument();

				doc.LoadHtml(OriginalHtml);

				if (doc.ParseErrors.Any())
				{
					//Invalid HTML
					ValidationErrors = doc.ParseErrors;
					return false;
				}
			}
			ValidationErrors = new List<HtmlParseError>();
			return true;
		}

		/// <summary>
		/// Removes all &lt;script&gt; tags from HTML
		/// </summary>
		/// <param name="OriginalHtml"></param>
		/// <param name="ReplaceWith">optional - text or HTML to replace the script tag with</param>
		/// <returns></returns>
		public static string StripScripts(this string OriginalHtml, string ReplaceWith = "")
		{
			if (!OriginalHtml.IsNullOrWhiteSpace())
			{
				HtmlDocument doc = new HtmlDocument();

				doc.LoadHtml(OriginalHtml);

				var badNodes = doc.DocumentNode.SelectNodes("//script");
				if (badNodes != null)
				{
					if (ReplaceWith != "")
					{
						HtmlNode replacementNode = HtmlNode.CreateNode(ReplaceWith);

						foreach (var node in badNodes)
						{
							doc.DocumentNode.ReplaceChild(replacementNode, node);
						}
					}
					else
					{
						//Just remove
						foreach (var node in badNodes)
						{
							node.Remove();
						}
					}

					return doc.DocumentNode.InnerHtml.ToString();
				}
				else
				{
					//No scripts, just return original
					return OriginalHtml;
				}
			}
			else
			{
				return "";
			}
		}

		/// <summary>
		/// Removes all &lt;script&gt; tags from HTML
		/// </summary>
		/// <param name="OriginalHtml"></param>
		/// <param name="ReplaceWith">optional - text or HTML to replace the script tag with</param>
		/// <returns></returns>
		public static HtmlString StripScripts(this HtmlString OriginalHtml, string ReplaceWith = "")
		{
			var originalHtml = OriginalHtml.ToString();
			var finalHtml = originalHtml.StripScripts(ReplaceWith);

			return new HtmlString(finalHtml);
		}

		/// <summary>
		/// Removes all &lt;iframe&gt; tags from HTML
		/// </summary>
		/// <param name="OriginalHtml"></param>
		/// <param name="ReplaceWith">optional - text or HTML to replace the script tag with</param>
		/// <returns></returns>
		public static string StripIframes(this string OriginalHtml, string ReplaceWith = "")
		{
			if (!OriginalHtml.IsNullOrWhiteSpace())
			{
				HtmlDocument doc = new HtmlDocument();

				doc.LoadHtml(OriginalHtml);

				var badNodes = doc.DocumentNode.SelectNodes("//iframe");
				if (badNodes != null)
				{
					if (ReplaceWith != "")
					{
						HtmlNode replacementNode = HtmlNode.CreateNode(ReplaceWith);

						foreach (var node in badNodes)
						{
							doc.DocumentNode.ReplaceChild(replacementNode, node);
						}
					}
					else
					{
						//Just remove
						foreach (var node in badNodes)
						{
							node.Remove();
						}
					}
					return doc.DocumentNode.InnerHtml.ToString();
				}
				else
				{
					//Nothing to remove, just return original
					return OriginalHtml;
				}
			}
			else
			{
				return "";
			}
		}

		/// <summary>
		/// Removes all &lt;iframe&gt; tags from HTML
		/// </summary>
		/// <param name="OriginalHtml"></param>
		/// <param name="ReplaceWith">optional - text or HTML to replace the script tag with</param>
		/// <returns></returns>
		public static HtmlString StripIframes(this HtmlString OriginalHtml, string ReplaceWith = "")
		{
			var originalHtml = OriginalHtml.ToString();
			var finalHtml = originalHtml.StripIframes(ReplaceWith);

			return new HtmlString(finalHtml);
		}

		/// <summary>
		/// Removes all &lt;p&gt; tags from HTML
		/// </summary>
		/// <param name="OriginalHtml"></param>
		/// <param name="ReplaceWithBr">optional - if there are multiple paragraphs, will put a &lt;br/&gt; tag between them</param>
		/// <returns></returns>
		public static string StripParagraphTags(this string OriginalHtml, bool ReplaceWithBr = true)
		{
			if (!OriginalHtml.IsNullOrWhiteSpace())
			{
				var finalHtml = "";
				HtmlDocument doc = new HtmlDocument();

				doc.LoadHtml(OriginalHtml);

				var badNodes = doc.DocumentNode.SelectNodes("//p");

				if (badNodes != null && badNodes.Any())
				{
					var totalParagraphs = badNodes.Count;

					foreach (var node in badNodes)
					{
						var innerText = node.InnerHtml;
						var newHtml = "";
						var isLastParagraph = badNodes.IndexOf(node) == (totalParagraphs - 1);

						if (ReplaceWithBr & totalParagraphs > 1 & !isLastParagraph)
						{
							newHtml = $"{innerText} <br/>";
						}
						else
						{
							newHtml = $"{innerText} ";
						}

						finalHtml += newHtml;
						// HtmlNode replacementNode = HtmlNode.CreateNode(newHtml);
						// doc.DocumentNode.ReplaceChild(replacementNode, node);
					}

					return finalHtml;
					//return doc.DocumentNode.InnerHtml.ToString();
				}
				else
				{
					//Nothing to remove, just return original
					return OriginalHtml;
				}
			}
			else
			{
				return "";
			}
		}

		/// <summary>
		/// Removes all &lt;p&gt; tags from HTML
		/// </summary>
		/// <param name="OriginalHtml"></param>
		/// <param name="ReplaceWithBr">optional - if there are multiple paragraphs, will put a &lt;br/&gt; tag between them</param>
		/// <returns></returns>
		public static HtmlString StripParagraphTags(this HtmlString? OriginalHtml, bool ReplaceWithBr = true)
		{
			var finalHtml = "";

			if (OriginalHtml != null)
			{
				var originalHtml = OriginalHtml.ToString();
				finalHtml = originalHtml.StripParagraphTags(ReplaceWithBr);
			}

			return new HtmlString(finalHtml);
		}

		#endregion
	}

}
