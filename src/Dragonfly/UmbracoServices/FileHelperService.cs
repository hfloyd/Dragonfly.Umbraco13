namespace Dragonfly.UmbracoServices
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Net;
	using System.Text;
	using System.Threading;
	using Dragonfly.NetModels;
	using Umbraco.Cms.Core.Hosting;

	/// <summary>
	/// Helpers to handle File I/O
	/// </summary>
	[Obsolete("Use 'Dragonfly.NetHelperServices.FileHelperService'")]
	public class FileHelperService
	{
		private readonly IHostingEnvironment _hostingEnvironment;
		private readonly string _MappedRootPath;


		//TODO: Remove this file in a future version
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public FileHelperService(IHostingEnvironment hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment;
			_MappedRootPath = hostingEnvironment.MapPathWebRoot("/");
		}

		#region Retrieve Remote Files (HTTP)

		/// <summary>
		/// Get a file from a url and save it to the filesystem.
		/// </summary>
		/// <param name="FileUrl">Http url of file to save</param>
		/// <param name="SaveLocationFolder">disk folder where the file should be saved (virtual to root)</param>
		/// <param name="SaveFileName">Desired filename for saved file</param>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public void DownloadAndSaveHttpFile(string FileUrl, string SaveLocationFolder, string SaveFileName)
		{
			string saveLocation = string.Concat(SaveLocationFolder, "/", SaveFileName);

			DownloadAndSaveHttpFile(FileUrl, saveLocation);
		}

		/// <summary>
		/// Get a file from a url and save it to the filesystem.
		/// </summary>
		/// <param name="FileUrl">Http url of file to save</param>
		/// <param name="SaveLocation">Disk location (incl. filename) where the file should be saved (virtual to web root)</param>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public void DownloadAndSaveHttpFile(string FileUrl, string SaveLocation)
		{
			string remoteUrl = FileUrl;
			string serverPath = "";

			try
			{
				serverPath = _hostingEnvironment.MapPathWebRoot(SaveLocation);
			}
			catch (Exception exMapPath)
			{
				//TODO: Error handling/logging
				serverPath = SaveLocation;
			}

			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteUrl);
				request.Method = WebRequestMethods.Http.Get;

				//Stream fileResponseStream;

				HttpWebResponse fileResponse = (HttpWebResponse)request.GetResponse();

				//fileResponseStream = fileResponse.GetResponseStream();

				using (Stream writeStream = File.OpenWrite(serverPath))
				using (Stream fileResponseStream = fileResponse.GetResponseStream())
				{
					fileResponseStream.CopyTo(writeStream);
				}
			}
			catch (Exception ex)
			{
				//TODO: Error handling/logging
			}
		}

		#endregion

		#region FTP
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public static string[] GetFtpFileList(string FtpHostServer, string FtpUserName, string FtpPassword)
		{
			string[] downloadFiles;
			StringBuilder result = new StringBuilder();
			WebResponse response = null;
			StreamReader reader = null;
			try
			{
				FtpWebRequest reqFTP;
				reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + FtpHostServer + "/"));
				reqFTP.UseBinary = true;
				reqFTP.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
				reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
				reqFTP.Proxy = null;
				reqFTP.KeepAlive = false;
				reqFTP.UsePassive = false;
				response = reqFTP.GetResponse();
				reader = new StreamReader(response.GetResponseStream());
				string line = reader.ReadLine();
				while (line != null)
				{
					result.Append(line);
					result.Append("\n");
					line = reader.ReadLine();
				}
				// to remove the trailing '\n'
				result.Remove(result.ToString().LastIndexOf('\n'), 1);
				return result.ToString().Split('\n');
			}
			catch (Exception ex)
			{
				if (reader != null)
				{
					reader.Close();
				}
				if (response != null)
				{
					response.Close();
				}
				downloadFiles = null;
				return downloadFiles;
			}
		}


		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public bool DownloadFtpFile(string FtpHostServer, string FtpUserName, string FtpPassword, string FtpDirectoryPath, string FtpFileName, string SaveLocationPath, string SaveFileName)
		{
			string remoteUrl = "ftp://" + FtpHostServer + "/" + FtpDirectoryPath + "/" + FtpFileName;

			string filePath = FtpDirectoryPath + "/" + FtpFileName;
			string serverPath = "";
			try
			{
				serverPath = _hostingEnvironment.MapPathWebRoot(SaveLocationPath);
			}
			catch (Exception exMapPath)
			{
				//TODO: Update using new code pattern:

				serverPath = SaveLocationPath;
			}

			string FullSaveLocation = String.Concat(serverPath, "\\", SaveFileName);
			//TODO: Update using new code pattern:
			//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
			//var msg = string.Format("");
			//Info.LogInfo("Files.DownloadFtpFile: FullSaveLocation=" + FullSaveLocation);

			//Test that server can be accessed
			//TODO: Update using new code pattern:
			//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
			//var msg = string.Format("");
			//Info.LogInfo("Files.DownloadFtpFile: FtpServerStatus=" + FtpServerStatus(FtpHostServer, FtpUserName, FtpPassword));
			//Info.LogInfo("Files.DownloadFtpFile: FtpDirectoryStatus=" + FtpDirectoryStatus(FtpHostServer, FtpUserName, FtpPassword, FtpDirectoryPath));

			FileStream writeStream = new FileStream(FullSaveLocation, FileMode.Create);
			try
			{
				long length = GetFileLength(FtpHostServer, FtpUserName, FtpPassword, filePath, true);
				//TODO: Update using new code pattern:
				//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
				//var msg = string.Format("");
				//Info.LogInfo("Files.DownloadFtpFile: GetFileLength(RemoteURL)=" + length);
				long offset = 0;
				int retryCount = 10;
				int? readTimeout = 5 * 60 * 1000; //five minutes

				while (retryCount > 0)
				{
					using (Stream responseStream = GetFileAsStream(remoteUrl, FtpUserName, FtpPassword, true, offset, requestTimeout: readTimeout != null ? readTimeout.Value : Timeout.Infinite))
					{
						//TODO: Update using new code pattern:
						//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
						//var msg = string.Format("");
						//Info.LogInfo("Files.DownloadFtpFile: GetFileAsStream(RemoteURL).length" + responseStream.Length);

						using (FileStream fileStream = new FileStream(FullSaveLocation, FileMode.Append))
						{
							byte[] buffer = new byte[4096];
							try
							{
								int bytesRead = responseStream.Read(buffer, 0, buffer.Length);

								while (bytesRead > 0)
								{
									fileStream.Write(buffer, 0, bytesRead);

									bytesRead = responseStream.Read(buffer, 0, buffer.Length);
								}

								return true;
							}
							catch (WebException exWeb)
							{
								// Do nothing - consume this exception to force a new read of the rest of the file

								//TODO: Update using new code pattern:
								//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
								//var msg = string.Format("");
								//Info.LogException("Files.DownloadFtpFile", webex, "HANDLED");
							}
						}

						//TODO: Update using new code pattern:
						//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
						//var msg = string.Format("");
						//Info.LogInfo("Files.DownloadFtpFile : File.Exists(FullSaveLocation)=", File.Exists(FullSaveLocation));

						if (File.Exists(FullSaveLocation))
						{
							offset = new FileInfo(FullSaveLocation).Length;
						}
						else
						{
							offset = 0;
						}

						retryCount--;

						if (offset == length)
						{
							return true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				//TODO: Update using new code pattern:
				//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
				//var msg = string.Format("");
				//Info.LogException("Files.DownloadFtpFile", ex);
			}

			return false;
		}
	
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		private static string FtpDirectoryStatus(string FtpHostServer, string username, string password, string FtpFolderPathToTest = "")
		{
			string ReturnMsg = "";
			string FullDirToTest = "ftp://" + FtpHostServer + "/" + FtpFolderPathToTest;
			//TODO: Update using new code pattern:
			//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
			//var msg = string.Format("");
			//Info.LogInfo("Files.FtpDirectoryExists: FullDirToTest=" + FullDirToTest);

			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FullDirToTest);
			request.Credentials = new NetworkCredential(username, password);
			string CredentialsInfo = request.Credentials.ToString();
			request.Method = WebRequestMethods.Ftp.ListDirectory;

			try
			{
				using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
				{
					// Okay.  
					ReturnMsg = "Server Connection Sucessful";
				}
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					FtpWebResponse response = (FtpWebResponse)ex.Response;
					ReturnMsg = response.StatusCode.ToString();

					if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
					{
						// Directory not found.  
						ReturnMsg += " - " + FullDirToTest;
					}
					else if (response.StatusCode == FtpStatusCode.NotLoggedIn)
					{
						// Directory not found.  
						ReturnMsg += " - " + CredentialsInfo;
					}

				}
				ReturnMsg = "Unknown";
			}
			return ReturnMsg;
		}

		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		private static string FtpServerStatus(string FtpHostServer, string username, string password)
		{
			string ReturnMsg = "";
			string ServerToTest = "ftp://" + FtpHostServer + "/";
			//TODO: Update using new code pattern:
			//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
			//var msg = string.Format("");
			//Info.LogInfo("Files.FtpServerStatus: ServerToTest=" + ServerToTest);

			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ServerToTest);
			request.Credentials = new NetworkCredential(username, password);
			string CredentialsInfo = request.Credentials.ToString();
			request.Method = WebRequestMethods.Ftp.ListDirectory;

			try
			{
				using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
				{
					// Okay.  
					ReturnMsg = "Server Connection Sucessful";
				}
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					FtpWebResponse response = (FtpWebResponse)ex.Response;
					ReturnMsg = response.StatusCode.ToString();

					if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
					{
						// Directory not found.  
						ReturnMsg += " - " + ServerToTest;
					}
					else if (response.StatusCode == FtpStatusCode.NotLoggedIn)
					{
						// Directory not found.  
						ReturnMsg += " - " + CredentialsInfo;
					}

				}
				ReturnMsg = "Unknown";
			}
			return ReturnMsg;
		}

		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		private static Stream GetFileAsStream(string ftpUrl, string username, string password, bool usePassive, long offset, int requestTimeout)
		{
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);

			request.KeepAlive = false;
			request.ReadWriteTimeout = requestTimeout;
			request.Timeout = requestTimeout;
			request.ContentOffset = offset;
			request.UsePassive = usePassive;
			request.UseBinary = true;

			request.Credentials = new NetworkCredential(username, password);

			request.Method = WebRequestMethods.Ftp.DownloadFile;

			Stream fileResponseStream;

			FtpWebResponse fileResponse = (FtpWebResponse)request.GetResponse();

			fileResponseStream = fileResponse.GetResponseStream();

			return fileResponseStream;
		}


		#endregion

		#region Create Files/Folders

		/// <summary>
		/// Will check for the existence of a directory on disk and create it if missing
		/// </summary>
		/// <param name="FolderPath">Path to directory</param>
		/// <returns>TRUE if sucessful</returns>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public bool CreateDirectoryIfMissing(string FolderPath)
		{
			bool success = false;

			string mappedFolderPath = "";
			var canGetPath = TryGetMappedPath(FolderPath, out mappedFolderPath);

			if (Directory.Exists(mappedFolderPath))
			{
				success = true;
			}
			else
			{
				try
				{
					Directory.CreateDirectory(mappedFolderPath);
					success = true;
				}
				catch (Exception ex)
				{
					//TODO: Update using new code pattern:
					//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
					//var msg = string.Format("");
					//Info.LogException("Files.CreateDirectoryIfMissing", ex, "[MappedFolderPath=" + MappedFolderPath + "]");
					success = false;
				}
			}
			return success;
		}

		/// <summary>
		/// Creates an empty file at a location, creating directories as needed
		/// </summary>
		/// <param name="FullFilePath">Path for directories and file</param>
		/// <returns>Filestream for new file</returns>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public static FileStream CreateFileAndDirectory(string FullFilePath)
		{
			string directoryName = Path.GetDirectoryName(FullFilePath);

			if (Directory.Exists(directoryName) == false)
			{
				Directory.CreateDirectory(directoryName);
			}

			FileStream fs = File.Create(FullFilePath);

			return fs;
		}

		/// <summary>
		/// Writes some text to a provided file location.
		/// </summary>
		/// <param name="FilePath">Virtual or Physical path - Inlcuding the desired filename with a text-compatible extension (ex: .txt, .xml, .json, etc.)</param>
		/// <param name="TextContent">Text to write to file</param>
		/// <param name="CreateDirectoryIfMissing">If the directories int he path don't exist, create them rather than failing</param>
		/// <param name="FailSilently">If TRUE won't throw an error on failure. Included for backward compatibility.</param>
		/// <returns></returns>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public bool CreateTextFile(string FilePath, string TextContent, bool CreateDirectoryIfMissing = false, bool FailSilently = true)
		{
			string mappedFilePath = "";
			var canGetPath = TryGetMappedPath(FilePath, out mappedFilePath);
			try
			{
				if (CreateDirectoryIfMissing)
				{
					string directoryName = Path.GetDirectoryName(GetMappedPath(FilePath));

					if (Directory.Exists(directoryName) == false)
					{
						Directory.CreateDirectory(directoryName);
					}
				}

				// WriteAllText creates a file, writes the specified string to the file,
				// and then closes the file.    You do NOT need to call Flush() or Close().
				System.IO.File.WriteAllText(mappedFilePath, TextContent);
			}
			catch (Exception ex)
			{
				if (!FailSilently)
				{
					//Pass error back up
					throw ex;
				}

				return false;
				//var msg = "";
				//if (ex.Message.Contains("path's format is not supported"))
				//{
				//    var functionName = string.Format("{0}.CreateTextFile", ThisClassName);
				//    if (mappedFilePath.Contains(":"))
				//    {msg = "Do you have a colon in the filename?";}
				//   // Info.LogException(functionName, ex, msg);
				//}
			}
			return true;
		}

		/// <summary>
		/// Writes text to a file
		/// </summary>
		/// <param name="FilePath">Path and filename</param>
		/// <param name="TextToWrite">Text content to add to file</param>
		/// <param name="Overwrite">If FALSE will just append as a line to existing file contents, TRUE will overwrite all file contents</param>
		/// <param name="PrefixWithTimestamp">Add a timestamp to the beginning of the line appended (useful for log files)</param>
		//TODO: Fix
		//public static void WriteToTextFile(string FilePath, string TextToWrite, bool Overwrite = false, bool PrefixWithTimestamp = true)
		//{
		//    string LogFilePath = "";
		//    try
		//    {
		//        LogFilePath = HttpContext.Current.Server.MapPath(FilePath);
		//    }
		//    catch (System.Web.HttpException exMapPath)
		//    {
		//        var functionName = string.Format("{0}.WriteToTextFile", ThisClassName);
		//        //Info.LogException(functionName, exMapPath, "(Error handled by Code)", true);
		//        LogFilePath = FilePath;
		//    }

		//    string textLine;

		//    if (PrefixWithTimestamp)
		//    {
		//        textLine = DateTime.Now + "---" + TextToWrite;
		//        //('yyyy-mm-dd-HH:MM:SS') + 
		//    }
		//    else
		//    {
		//        textLine = TextToWrite;
		//    }

		//    if (Overwrite == true | File.Exists(LogFilePath) == false)
		//    {
		//        FileStream fsNew = Files.CreateFileAndDirectory(LogFilePath);
		//        StreamWriter swNew = new StreamWriter(fsNew);
		//        swNew.WriteLine(textLine);
		//        swNew.Close();
		//        fsNew.Close();
		//    }
		//    else
		//    {
		//        StreamWriter swAppend = File.AppendText(LogFilePath);
		//        swAppend.WriteLine(textLine);
		//        swAppend.Close();
		//    }
		//}

		#endregion

		#region Read Files

		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public IEnumerable<string> ListLocalFiles(string FolderPath)
		{
			var mappedPath = "";
			var isMappable = TryGetMappedPath(FolderPath, out mappedPath);
			var files = System.IO.Directory.EnumerateFiles(mappedPath);

			return files;
		}

		/// <summary>
		/// Reads a Text file, returning contents as a string
		/// </summary>
		/// <param name="FilePath">Full path to file</param>
		/// <returns></returns>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public string GetTextFileContents(string FilePath)
		{
			var readText = "";
			var mappedFilePath = "";
			try
			{
				mappedFilePath = GetMappedPath(FilePath);
			}
			catch (System.ArgumentException e)
			{
				// "ClassName": "System.ArgumentException",
				// "Message": "The path appears to already be fully qualified.  Please remove the call to MapPath",
				if (e.Message.Contains("MapPath"))
				{
					mappedFilePath = FilePath;
				}
			}

			var fileExists = File.Exists(mappedFilePath);
			if (fileExists)
			{
				readText = File.ReadAllText(mappedFilePath);
			}
			else
			{
				//Try app root
				mappedFilePath = GetMappedPathAppRoot(FilePath);
				readText = File.ReadAllText(mappedFilePath);
			}

			return readText;
		}

		//public static bool DisplayFileFromServer(Uri serverUri)
		//{
		//    // The serverUri parameter should start with the ftp:// scheme. 
		//    if (serverUri.Scheme != Uri.UriSchemeFtp)
		//    {
		//        return false;
		//    }
		//    // Get the object used to communicate with the server.
		//    WebClient request = new WebClient();

		//    // This example assumes the FTP site uses anonymous logon.
		//    request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
		//    try
		//    {
		//        byte[] newFileData = request.DownloadData(serverUri.ToString());
		//        string fileString = System.Text.Encoding.UTF8.GetString(newFileData);
		//        Console.WriteLine(fileString);
		//    }
		//    catch (WebException e)
		//    {
		//        Console.WriteLine(e.ToString());
		//    }
		//    return true;
		//}

		#endregion

		#region Get File Information

		/// <summary>
		/// Checks whether a file exists on disk
		/// </summary>
		/// <param name="FullFilePath">Relative or Mapped Path</param>
		/// <returns>True if file found, false if not</returns>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public bool FileExists(string FullFilePath)
		{
			string mappedFilePath = "";
			try
			{
				mappedFilePath = _hostingEnvironment.MapPathWebRoot(FullFilePath);
			}
			catch (Exception exMapPath)
			{
				mappedFilePath = FullFilePath;
			}

			if (File.Exists(mappedFilePath))
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Convert bytes into a friendlier format
		/// </summary>
		/// <param name="Bytes">value of the file size in bytes</param>
		/// <param name="FormatString">Adjust the format string to your preferences. For example "{0:0.#}{1}" would show a single decimal place, and no space.</param>
		/// <returns></returns>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public static string GetFriendlyFileSize(double Bytes, string FormatString = "{0:0.##} {1}")
		{
			string[] sizes = { "B", "KB", "MB", "GB" };
			double len = Bytes;
			int order = 0;
			while (len >= 1024 && order + 1 < sizes.Length)
			{
				order++;
				len = len / 1024;
			}

			string result = string.Format(FormatString, len, sizes[order]);

			return result;
		}

		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public static Size GetJpegDimensions(string filename)
		{
			FileStream stream = null;
			BinaryReader rdr = null;
			try
			{
				stream = System.IO.File.OpenRead(filename);
				rdr = new BinaryReader(stream);
				// keep reading packets until we find one that contains Size info
				for (; ; )
				{
					byte code = rdr.ReadByte();
					if (code != 0xFF) throw new ApplicationException(
							   "Unexpected value in file " + filename);
					code = rdr.ReadByte();
					switch (code)
					{
						// filler byte
						case 0xFF:
							stream.Position--;
							break;
						// packets without data
						case 0xD0:
						case 0xD1:
						case 0xD2:
						case 0xD3:
						case 0xD4:
						case 0xD5:
						case 0xD6:
						case 0xD7:
						case 0xD8:
						case 0xD9:
							break;
						// packets with size information
						case 0xC0:
						case 0xC1:
						case 0xC2:
						case 0xC3:
						case 0xC4:
						case 0xC5:
						case 0xC6:
						case 0xC7:
						case 0xC8:
						case 0xC9:
						case 0xCA:
						case 0xCB:
						case 0xCC:
						case 0xCD:
						case 0xCE:
						case 0xCF:
							ReadBEUshort(rdr);
							rdr.ReadByte();
							ushort h = ReadBEUshort(rdr);
							ushort w = ReadBEUshort(rdr);
							return new System.Drawing.Size(w, h);
						// irrelevant variable-length packets
						default:
							int len = ReadBEUshort(rdr);
							stream.Position += len - 2;
							break;
					}
				}
			}
			finally
			{
				if (rdr != null) rdr.Close();
				if (stream != null) stream.Close();
			}
		}

		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		private static long GetFileLength(string FtpHostServer, string username, string password, string FtpFilePath, bool usePassive)
		{
			string RemoteURL = "ftp://" + FtpHostServer + "/" + FtpFilePath;

			FtpWebRequest requestServerTest = (FtpWebRequest)WebRequest.Create(FtpHostServer);
			requestServerTest.Credentials = new NetworkCredential(username, password);
			requestServerTest.Method = WebRequestMethods.Ftp.ListDirectory;
			FtpWebResponse ServerResponse = (FtpWebResponse)requestServerTest.GetResponse();



			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(RemoteURL);
			request.KeepAlive = false;
			request.UsePassive = usePassive;
			request.Credentials = new NetworkCredential(username, password);
			request.Method = WebRequestMethods.Ftp.GetFileSize;

			FtpWebResponse lengthResponse = (FtpWebResponse)request.GetResponse();
			long length = lengthResponse.ContentLength;
			lengthResponse.Close();
			return length;

		}

		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		private static ushort ReadBEUshort(BinaryReader rdr)
		{
			ushort hi = rdr.ReadByte();
			hi <<= 8;
			ushort lo = rdr.ReadByte();
			return (ushort)(hi | lo);
		}

		#endregion

		#region MapPath
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public string UnMapPath(string MappedPath)
		{
			string RootMapPath = GetMappedPath("/");
			string VirtualPath = "";


			VirtualPath = MappedPath.ToLower(); //start with the provided MappedPath, standardized to lowercase to make replacing easy.
			VirtualPath = VirtualPath.Replace(RootMapPath.ToLower(), ""); //Get rid of the portion to the website root

			string BackslashChar = @"\";
			VirtualPath = VirtualPath.Replace(BackslashChar, "/"); //flip the slashes

			return VirtualPath;
		}

		/// <summary>
		/// Get a mapped path of the provided path
		/// </summary>
		/// <param name="MappedOrRelativePath">The Path to look for</param>
		/// <returns>The Mapped path</returns>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public string GetMappedPath(string MappedOrRelativePath)
		{
			string mappedFolderPath = "";
			if (MappedOrRelativePath != null)
			{
				//Allow errors to bubble up
				//try
				//{
				mappedFolderPath = _hostingEnvironment.MapPathWebRoot(MappedOrRelativePath);
				//}
				//catch (Exception ex)
				//{
				//    //Try AppDomain
				//    var fixedPath = "";
				//    if (MappedOrRelativePath.StartsWith("~"))
				//    {
				//        fixedPath = MappedOrRelativePath.Replace("~/", "");
				//        MappedFolderPath = Path.Combine(HttpRuntime.AppDomainAppPath, fixedPath);
				//    }
				//    else if (MappedOrRelativePath.StartsWith("/"))
				//    {
				//        fixedPath = MappedOrRelativePath.TrimStart('/');
				//        MappedFolderPath = Path.Combine(HttpRuntime.AppDomainAppPath, fixedPath);
				//    }
				//else
				//{
				//    mappedFolderPath = MappedOrRelativePath;
				//    // }

				//}
			}

			return mappedFolderPath;
		}

		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public string GetMappedPathAppRoot(string MappedOrRelativePath)
		{
			string mappedFolderPath = "";
			if (MappedOrRelativePath != null)
			{
				//Allow errors to bubble up
				mappedFolderPath = _hostingEnvironment.MapPathContentRoot(MappedOrRelativePath);

			}

			return mappedFolderPath;
		}


		/// <summary>
		/// Tries to get a mapped path of the provided path
		/// </summary>
		/// <param name="MappedOrRelativePath">The Path to look for</param>
		/// <param name="MappedFolderPath">The Mapped path, returned</param>
		/// <returns>False, if an exception occurred</returns>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public bool TryGetMappedPath(string MappedOrRelativePath, out string MappedFolderPath)
		{
			MappedFolderPath = "";
			if (MappedOrRelativePath != null)
			{
				try
				{
					MappedFolderPath = _hostingEnvironment.MapPathWebRoot(MappedOrRelativePath);
				}
				catch (Exception ex)
				{
					//Try AppDomain
					//var fixedPath = "";
					//if (MappedOrRelativePath.StartsWith("~"))
					//{
					//    fixedPath = MappedOrRelativePath.Replace("~/", "");
					//    MappedFolderPath = Path.Combine(HttpRuntime.AppDomainAppPath, fixedPath);
					//}
					//else if (MappedOrRelativePath.StartsWith("/"))
					//{
					//    fixedPath = MappedOrRelativePath.TrimStart('/');
					//    MappedFolderPath = Path.Combine(HttpRuntime.AppDomainAppPath, fixedPath);
					//}
					//else
					//{
					MappedFolderPath = MappedOrRelativePath;
					return false;
					// }

				}
			}

			return true;
		}

		/// <summary>
		/// Tries to get a mapped path of the provided path
		/// </summary>
		/// <param name="MappedOrRelativePath">The Path to look for</param>
		/// <param name="MappedFolderPath">The Mapped path, returned</param>
		/// <returns>StatusMessage with information about the operation</returns>
		[Obsolete("This duplicates functionality in Dragonfly.NetHelperServices.FileHelperService - Use that library as this one will eventually be removed.")] //2023-01-27
		public StatusMessage TryGetMappedPathWithStatus(string MappedOrRelativePath, out string MappedFolderPath)
		{
			var status = new StatusMessage();
			MappedFolderPath = "";
			if (MappedOrRelativePath != null)
			{
				try
				{
					if (MappedOrRelativePath.Contains("\\"))
					{
						//Path is already a physical path - return it.
						MappedFolderPath = MappedOrRelativePath;
						status.Success = true;
					}
					else
					{
						//Try to Map it

						MappedFolderPath = _hostingEnvironment.MapPathWebRoot(MappedOrRelativePath);
						status.Success = true;

					}
				}
				catch (Exception ex)
				{
					status.RelatedException = ex;

					//Try AppDomain
					//var fixedPath = "";
					//if (MappedOrRelativePath.StartsWith("~"))
					//{
					//    fixedPath = MappedOrRelativePath.Replace("~/", "");
					//    MappedFolderPath = Path.Combine(HttpRuntime.AppDomainAppPath, fixedPath);
					//    status.Success = true;
					//}
					//else if (MappedOrRelativePath.StartsWith("/"))
					//{
					//    fixedPath = MappedOrRelativePath.TrimStart('/');
					//    MappedFolderPath = Path.Combine(HttpRuntime.AppDomainAppPath, fixedPath);
					//    status.Success = true;
					//}
					//else
					//{
					MappedFolderPath = MappedOrRelativePath;
					status.Success = false;
					status.Message = $"An error occurred and MappedOrRelativePath '{MappedOrRelativePath}' doesn't start with '~' or '/'";
					//  }
				}
			}

			return status;
		}

		#endregion

	}
}