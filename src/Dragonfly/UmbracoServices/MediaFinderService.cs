namespace Dragonfly.UmbracoServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dragonfly.UmbracoHelpers;
    using Dragonfly.UmbracoModels;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Web.Common;
 using Umbraco.Extensions;


    /// <summary>
    /// Provides tools to search and interact with the Media section
    /// </summary>
    public class MediaFinderService
    {
        #region Dependency Injection - Let's get some Umbraco goodies available here!
        private readonly UmbracoHelper _umbracoHelper;
        private readonly ILogger _logger;
        #endregion

        //private IEnumerable<IPublishedContent> _allMediaFlat;
        private IEnumerable<IPublishedContent> _mediaAtRoot = new List<IPublishedContent>();
        private Dictionary<int, IEnumerable<SimpleMediaInfo>> _simpleMediaDict = new Dictionary<int, IEnumerable<SimpleMediaInfo>>();

        /// <summary>
        /// Service to retrieve Media Nodes via various search methods
        /// </summary>
        /// <param name="umbHelper">UmbracoHelper passed-in</param>
        public MediaFinderService(UmbracoHelper UmbHelper, ILogger<MediaFinderService> Logger)
        {
            _umbracoHelper = UmbHelper;
            _logger = Logger;
        }
        

        #region All Media Nodes (SimpleMediaInfo)

        /// <summary>
        /// Gets all media nodes as SimpleMediaInfo
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SimpleMediaInfo> GetAllSimpleMediaNodes()
        {
            var nodesList = new List<SimpleMediaInfo>();

            //Get nodes as IPublishedContent
            var topLevelNodes = _umbracoHelper.MediaAtRoot().OrderBy(n => n.SortOrder);

            foreach (var thisNode in topLevelNodes)
            {
                nodesList.AddRange(LoopSimpleMediaNodes(thisNode));
            }

            return nodesList;
        }

        /// <summary>
        /// Gets all media nodes as SimpleMediaInfo
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SimpleMediaInfo> GetAllSimpleMediaNodes(int StartNodeId)
        {
            var nodesList = new List<SimpleMediaInfo>();

            //Get nodes as IPublishedContent
            var topLevelNodes = _umbracoHelper.Media(StartNodeId).AsEnumerableOfOne();

            foreach (var thisNode in topLevelNodes)
            {
                nodesList.AddRange(LoopSimpleMediaNodes(thisNode));
            }

            return nodesList;
        }

        internal static IEnumerable<SimpleMediaInfo> LoopSimpleMediaNodes(IPublishedContent ThisNode)
        {
            var nodesList = new List<SimpleMediaInfo>();

            //Add current node, then loop for children
            try
            {
                nodesList.Add(new SimpleMediaInfo(ThisNode));

                if (ThisNode.Children().Any())
                {
                    foreach (var childNode in ThisNode.Children().OrderBy(n => n.SortOrder))
                    {
                        nodesList.AddRange(LoopSimpleMediaNodes(childNode));
                    }
                }
            }
            catch (Exception e)
            {
                //skip
            }

            return nodesList;
        }

        #endregion

        #region Get By Name

        /// <summary>
        /// Lookup Media Image by Node Name
        /// </summary>
        /// <param name="ImageMediaName">Name to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetImageByName(string ImageMediaName, bool CaseSensitive = true, int StartNodeId = 0)
        {
            return GetMediaByName(ImageMediaName, CaseSensitive, StartNodeId, Constants.Conventions.MediaTypes.Image);
        }

        /// <summary>
        /// Lookup Media Folder by Node Name
        /// </summary>
        /// <param name="FolderName">Name to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetFolderByName(string FolderName, bool CaseSensitive = true, int StartNodeId = 0)
        {
            return GetMediaByName(FolderName, CaseSensitive, StartNodeId, Constants.Conventions.MediaTypes.Folder);
        }

        /// <summary>
        /// Lookup Media File by Node Name
        /// </summary>
        /// <param name="FileMediaName">Name to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetFileByName(string FileMediaName, bool CaseSensitive = true, int StartNodeId = 0)
        {
            return GetMediaByName(FileMediaName, CaseSensitive, StartNodeId, Constants.Conventions.MediaTypes.File);
        }

        /// <summary>
        /// Lookup Media Node by Node Name
        /// </summary>
        /// <param name="MediaName">Name to search for</param>
        /// <param name="CaseSensitive">Match case exactly</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <param name="MediaTypeAlias">Alias of MediaType to return</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetMediaByName(string MediaName, bool CaseSensitive = true, int StartNodeId = 0, string MediaTypeAlias = "")
        {
            var allMediaList = new List<IPublishedContent>();

            if (StartNodeId > 0)
            {
                var startMedia = _umbracoHelper.Media(StartNodeId);
                allMediaList.AddRange(FindDescendantsByName(startMedia, MediaName, CaseSensitive));
            }
            else
            {
                var rootMedia = GetMediaAtRoot().ToList();

                if (rootMedia.Any())
                {
                    foreach (var mediaRoot in rootMedia)
                    {
                        allMediaList.AddRange(FindDescendantsByName(mediaRoot, MediaName, CaseSensitive));
                    }
                }
            }

            if (MediaTypeAlias != "")
            {
                var limitedMediaList = allMediaList.Where(n => n.ContentType.Alias == MediaTypeAlias);
                return limitedMediaList;
            }
            else
            {
                return allMediaList;
            }

        }

        private IEnumerable<IPublishedContent> FindDescendantsByName(IPublishedContent StartMedia, string MediaName,
	        bool CaseSensitive=true)
        {
            //var mediaList = new List<IPublishedContent>();
            if (CaseSensitive)
            {
	            return StartMedia.DescendantsOrSelf().Where(n => n.Name == MediaName);
            }
            else
            {
				return StartMedia.DescendantsOrSelf().Where(n => n.Name.ToLower() == MediaName.ToLower());
			}
        }

        #endregion

        #region Get By File Path

        /// <summary>
        /// Lookup Media Image by File Path
        /// </summary>
        /// <param name="MediaFilePath">File Path to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetImageByFilePath(string MediaFilePath, int StartNodeId = 0)
        {
            return GetMediaByFilePath(MediaFilePath, StartNodeId, Constants.Conventions.MediaTypes.Image);
        }

        /// <summary>
        /// Lookup Media File by File Path
        /// </summary>
        /// <param name="MediaFilePath">File Path to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetFileByFilePath(string MediaFilePath, int StartNodeId = 0)
        {
            return GetMediaByFilePath(MediaFilePath, StartNodeId, Constants.Conventions.MediaTypes.File);
        }

        /// <summary>
        /// Lookup Media Node by File Path
        /// </summary>
        /// <param name="MediaFilePath">File Path to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <param name="MediaTypeAlias">Alias of MediaType to return</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetMediaByFilePath(string MediaFilePath, int StartNodeId = 0, string MediaTypeAlias = "")
        {
            var allMediaList = new List<IPublishedContent>();

            if (StartNodeId > 0)
            {
                var startMedia = _umbracoHelper.Media(StartNodeId);
                allMediaList.AddRange(FindDescendantsByFilePath(startMedia, MediaFilePath));
            }
            else
            {
                var rootMedia = GetMediaAtRoot().ToList();

                if (rootMedia.Any())
                {
                    foreach (var mediaRoot in rootMedia)
                    {
                        allMediaList.AddRange(FindDescendantsByFilePath(mediaRoot, MediaFilePath));
                    }
                }
            }

            if (MediaTypeAlias != "")
            {
                var limitedMediaList = allMediaList.Where(n => n.ContentType.Alias == MediaTypeAlias);
                return limitedMediaList;
            }
            else
            {
                return allMediaList;
            }

        }

        private IEnumerable<IPublishedContent> FindDescendantsByFilePath(IPublishedContent StartMedia, string MediaPath)
        {
            //var mediaList = new List<IPublishedContent>();

            return StartMedia.DescendantsOrSelf().Where(n => n.Value<string>("umbracoFile") == MediaPath);
        }

        #endregion

        #region Get By File Name

        /// <summary>
        /// Lookup Media Image by File Path
        /// </summary>
        /// <param name="MediaFilePath">File Path to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<SimpleMediaInfo> GetImageInfoByFilename(string MediaFilename, int StartNodeId = 0)
        {
            return GetMediaInfoByFilename(MediaFilename, StartNodeId, Constants.Conventions.MediaTypes.Image);
        }

        /// <summary>
        /// Lookup Media File by File Path
        /// </summary>
        /// <param name="MediaFilePath">File Path to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<SimpleMediaInfo> GetFileInfoByFilename(string MediaFilename, int StartNodeId = 0)
        {
            return GetMediaInfoByFilename(MediaFilename, StartNodeId, Constants.Conventions.MediaTypes.File);
        }

        /// <summary>
        /// Lookup Media Node by File Path
        /// </summary>
        /// <param name="MediaFilePath">File Path to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <param name="MediaTypeAlias">Alias of MediaType to return</param>
        /// <returns></returns>
        public IEnumerable<SimpleMediaInfo> GetMediaInfoByFilename(string MediaFilename, int StartNodeId = 0, string MediaTypeAlias = "")
        {
            var allMediaList = GetSimpleMediaList(StartNodeId);
            var matchingMedia = allMediaList.Where(n => n.Filename == MediaFilename).ToList();

            if (MediaTypeAlias != "")
            {
                var limitedMediaList = matchingMedia.Where(n => n.DocumentTypeAlias == MediaTypeAlias);
                return limitedMediaList;
            }
            else
            {
                return matchingMedia;
            }

        }


        #endregion

        #region GetMediaLists

        private IEnumerable<IPublishedContent> GetMediaAtRoot()
        {
            if (!_mediaAtRoot.Any())
            {
                _mediaAtRoot = _umbracoHelper.MediaAtRoot();
            }

            return _mediaAtRoot;
        }

        private IEnumerable<SimpleMediaInfo> GetSimpleMediaList(int StartNode)
        {
            if (_simpleMediaDict.ContainsKey(StartNode))
            {
                return _simpleMediaDict[StartNode];
            }
            else
            {
                //Fill with list
                if (StartNode == 0)
                {
                    var nodes = GetAllSimpleMediaNodes().ToList();
                    _simpleMediaDict.Add(0, nodes);

                    return nodes;
                }
                else
                {
                    var nodes = GetAllSimpleMediaNodes(StartNode).ToList();
                    _simpleMediaDict.Add(StartNode, nodes);

                    return nodes;
                }

            }
        }

        private IEnumerable<SimpleMediaInfo> GetSimpleMediaList()
        {
            if (_simpleMediaDict.ContainsKey(0))
            {
                return _simpleMediaDict[0];
            }
            else
            {
                //Fill with list
                var nodes = GetAllSimpleMediaNodes().ToList();
                _simpleMediaDict.Add(0, nodes);

                return nodes;
            }
        }

        #endregion

    }


    /// <summary>
    /// Used to get information about Media
    /// </summary>
    public class SimpleMediaInfo
    {
	    public SimpleMediaInfo()
	    {

	    }

	    public SimpleMediaInfo(IPublishedContent Node)
	    {
		    this.Id = Node.Id;
		    this.Guid = Node.Key;
		    this.Udi = Node.ToUdi();
		    this.Name = Node.Name;
		    this.Path = Node.Path;
		    this.CreateDate = Node.CreateDate;
		    this.CreatorId = Node.CreatorId;
		    this.CreatorName = Node.CreatorName();
		    this.UpdateDate = Node.UpdateDate;
		    this.DocumentTypeAlias = Node.ContentType.Alias;
		    this.FilePath = !string.IsNullOrEmpty(Node.Value<string>("umbracoFile")) ? Node.Value<string>("umbracoFile") : "";
		    this.Filename = GetFileNameOnly(FilePath);
		    this.Url = Node.Url();
	    }

	    public Udi? Udi { get; set; }

	    public string Name { get; set; } = "";
	    public int Id { get; set; } = 0;
	    public Guid Guid { get; set; }
	    public string DocumentTypeAlias { get; set; } = "";
		public string Path { get; set; } = "";
		public string Url { get; set; } = "";
		public string FilePath { get; set; } = "";
		public string Filename { get; set; } = "";
		public string CreatorName { get; set; } = "";
		public int CreatorId { get; set; } = 0;
		public DateTime CreateDate { get; set; }=DateTime.MinValue;
	    public DateTime UpdateDate { get; set; } = DateTime.MinValue;

		private string GetFileNameOnly(string Path)
	    {
		    if (!string.IsNullOrEmpty(Path))
		    {
			    var structure = Path.Split('/');
			    var name = structure.Last();
			    return name;
		    }
		    else
		    {
			    return "";
		    }
	    }
    }
}
