using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using DocumentFormat.OpenXml.Math;
using static TPM_QAS.Models.AzureBlob;
using TPM_QAS.DAL;
using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System.IO;
using static Azure.Core.HttpHeader;
using TPM_QAS.Helpers;
using System.Security.Policy;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Helpers
{
    public class AzureBlobHelper
    {
        readonly ErrorLogSys errorLog = new ErrorLogSys();
        CommonFunction common = new CommonFunction();

        public async Task<Dictionary<string, string>> uploadBlobpdf(string path, string container, byte[] fileBytes)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(
                    new Uri(AzureBlobString.blobServiceEndpoint),
                    new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key));

                // Get or create container
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
                if (await containerClient.ExistsAsync() == false)
                {
                    containerClient = await blobServiceClient.CreateBlobContainerAsync(container);
                }

                // Get blob reference
                BlobClient blobClient = containerClient.GetBlobClient(path);

                // Upload from byte array
                using (var ms = new MemoryStream(fileBytes))
                {
                    await blobClient.UploadAsync(ms, overwrite: true);
                }

                dic.Add(path + ".pdf", blobClient.Uri.AbsoluteUri);
                return dic;
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return new Dictionary<string, string>();
            }
        }

        public async Task<Dictionary<string, string>> uploadBlob(string path, string container, IFormFile file)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            try
            {
                BlobServiceClient blobServiceClient;
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                
                blobServiceClient = new BlobServiceClient(
                                        new Uri(AzureBlobString.blobServiceEndpoint),
                                        new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key));

                // Get a reference to a container
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
                if (await containerClient.ExistsAsync() == false)
                {
                    //create container
                    containerClient = await blobServiceClient.CreateBlobContainerAsync(container);
                }

                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(path);

                // Upload the file
                using (var fileStream = file.OpenReadStream())
                {
                    if (fileStream.CanSeek)
                    {
                        fileStream.Position = 0;
                    }

                    await blobClient.UploadAsync(fileStream, true);
                }
                dic.Add(file.FileName, blobClient.Uri.AbsoluteUri);
                //await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

                //// Set the permissions so the blobs are public
                //await containerClient.SetAccessPolicyAsync();



                return dic;
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                //await errorLog.ErrorLog_Add_V2("uploadBlob", ex, "admin", "");
                return new Dictionary<string, string>();
            }

        }

        public async Task<string> DeleteBlobAsync(string uri)
        {
            try
            {
                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                string blobpath = Path.Combine(endpoint, container);
                uri = blobpath + "/" + uri;

                BlobServiceClient blobServiceClient;
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                
                blobServiceClient = new BlobServiceClient(
                                       new Uri(AzureBlobString.blobServiceEndpoint),
                                       new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key));

                Uri blobUri = new Uri(uri);
                var blobUriParts = new BlobUriBuilder(blobUri);

                string containerName = blobUriParts.BlobContainerName;
                string blobName = blobUriParts.BlobName;

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();


                return "Delete Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> DeleteAllBlobAsync(string filePath, string container)
        {
            try
            {
                BlobServiceClient blobServiceClient;
                HttpClientHandler httpClientHandler = new HttpClientHandler();

                blobServiceClient = new BlobServiceClient(
                                        new Uri(AzureBlobString.blobServiceEndpoint),
                                        new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key));

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

                foreach (BlobItem blobItem in containerClient.GetBlobs(prefix: filePath))
                {
                    // Get the blob client
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);

                    // Delete the blob
                    await blobClient.DeleteIfExistsAsync();
                }


                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> CopyBlobAsync(string sourceUri, string destinationContainer, string destinationPath)
        {
            try
            {
                BlobServiceClient blobServiceClient;
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                
                blobServiceClient = new BlobServiceClient(
                        new Uri(AzureBlobString.blobServiceEndpoint),
                        new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key));

                // Parse source URI
                Uri sourceBlobUri = new Uri(sourceUri);
                var blobUriBuilder = new BlobUriBuilder(sourceBlobUri);
                string sourceContainerName = blobUriBuilder.BlobContainerName;
                string sourceBlobName = blobUriBuilder.BlobName;

                // Get source and destination clients
                BlobContainerClient sourceContainerClient = blobServiceClient.GetBlobContainerClient(sourceContainerName);
                BlobClient sourceBlobClient = sourceContainerClient.GetBlobClient(sourceBlobName);

                BlobContainerClient destContainerClient = blobServiceClient.GetBlobContainerClient(destinationContainer);
                if (!await destContainerClient.ExistsAsync())
                {
                    await blobServiceClient.CreateBlobContainerAsync(destinationContainer);
                }
                BlobClient destBlobClient = destContainerClient.GetBlobClient(destinationPath);

                // Copy the blob
                await destBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);

                return destBlobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                //await errorLog.ErrorLog_Add_V2("CopyBlobAsync", ex, "admin", "");
                return ex.Message;
            }
        }

        public async Task<(Stream Stream, string FileName)> GetBlobFileSel(string fileUrl)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            
            using (HttpClient client = new HttpClient(handler: httpClientHandler, disposeHandler: true))
            {
                var response = await client.GetAsync(fileUrl);
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();

                string fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
                return (stream, fileName);
            }
        }


        public async Task<string> SetBlobUrlToken(string url)
        {
            try
            {
                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                //string blobpath = Path.Combine(endpoint, container);
                string blobpath = $"{endpoint.TrimEnd('/')}/{container}";

                url = blobpath + "/" + url;

                string originalUrl = HttpUtility.UrlDecode(url);
                BlobServiceClient blobServiceClient;
                HttpClientHandler httpClientHandler = new HttpClientHandler();

                blobServiceClient = new BlobServiceClient(
                                       new Uri(AzureBlobString.blobServiceEndpoint),
                                       new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key));

                // Get a reference to a container
                //string container = url.Replace(AzureBlobString.blobServiceEndpoint, "").Split('/')[0]; 

                //string blobName = originalUrl.Replace(AzureBlobString.blobServiceEndpoint + container + "/", "");
                string blobName = url.Substring(url.IndexOf(container) + container.Length + 1);


                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // ✅ Check if file exists in Azure Blob
                var fileExists = await blobClient.ExistsAsync();
                if (!fileExists.Value)
                {
                    return null; // or return a default icon/placeholder URL
                }

                string fileName = originalUrl.Split('/').Last();
                // Generate the SAS token
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTimeOffset.UtcNow.AddDays(365),
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                sasBuilder.ContentDisposition = "inline";
                sasBuilder.ContentType = getContentType(fileName);



                string sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)).ToString();
                UriBuilder sasUriBuilder = new UriBuilder(blobClient.Uri)
                {
                    Query = sasToken
                };

                return sasUriBuilder.Uri.ToString();

            }
            catch (Exception ex)
            {
                var a = ex.Message;
                return a;
            }
        }

        public string getContentType(string fileName)
        {
            string extension = System.IO.Path.GetExtension(fileName)?.ToLower();
            switch (extension)
            {
                // Image file types
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                case ".svg":
                    return "image/svg+xml";
                case ".webp":
                    return "image/webp";

                // Video file types
                case ".mp4":
                    return "video/mp4";
                case ".avi":
                    return "video/x-msvideo";
                case ".mov":
                    return "video/quicktime";
                case ".wmv":
                    return "video/x-ms-wmv";
                case ".flv":
                    return "video/x-flv";
                case ".mkv":
                    return "video/x-matroska";
                case ".webm":
                    return "video/webm";

                // Document file types
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                // Email file type
                case ".eml":
                    return "message/rfc822";

                // Default to binary content type
                default:
                    return "application/octet-stream";
            }
        }

        #region 2 layer folder

        public async Task<Dictionary<string, List<string>>> GetFolderAndFiles(string rootFolder)
        {
            var result = new Dictionary<string, List<string>>();

            try
            {
                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                BlobServiceClient blobServiceClient = new BlobServiceClient(
                    new Uri(endpoint),
                    new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                );

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

                // ensure trailing slash
                if (!rootFolder.EndsWith("/"))
                    rootFolder += "/";

                // get all blobs under rootFolder
                var blobs = containerClient.GetBlobs(prefix: rootFolder);

                foreach (var blobItem in blobs)
                {
                    string blobName = blobItem.Name; // e.g. INT_IDS/20250428/abc.pdf

                    if (string.IsNullOrEmpty(blobName) || blobName.EndsWith("/"))
                        continue;

                    // split by '/'
                    var parts = blobName.Split('/');

                    // we expect format: INT_IDS / [SECOND_LEVEL] / [FILE]
                    if (parts.Length >= 3)
                    {
                        string secondFolder = parts[1];   // 20250428
                        string fileName = parts[2];       // abc.pdf

                        if (!result.ContainsKey(secondFolder))
                            result[secondFolder] = new List<string>();

                        result[secondFolder].Add(fileName);
                    }
                }

                // optional: sort folders descending by date if format is yyyymmdd
                result = result
                    .OrderByDescending(k => k.Key)
                    .ToDictionary(k => k.Key, v => v.Value);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve second-level folders and files: " + ex.Message, ex);
            }
        }

        public async Task<List<(string FileName, string FileUrl)>> GetFilesInSubfolder(string subFolder, string rootFolder)
        {
            // to get file in second folder : container/folder 1/ folder 2/ file
            var result = new List<(string FileName, string FileUrl)>();

            try
            {
                // --- Validate input ---
                if (string.IsNullOrEmpty(rootFolder))
                    throw new ArgumentException("Root folder cannot be null or empty.");

                if (string.IsNullOrEmpty(subFolder))
                    throw new ArgumentException("Subfolder cannot be null or empty.");

                // --- Setup Azure Blob Client ---
                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                BlobServiceClient blobServiceClient = new BlobServiceClient(
                    new Uri(endpoint),
                    new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                );

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

                // --- Construct the prefix path ---
                string prefix = $"{rootFolder.TrimEnd('/')}/{subFolder.TrimEnd('/')}/";

                // --- List all blobs under that path ---
                var blobs = containerClient.GetBlobs(prefix: prefix);

                foreach (var blobItem in blobs)
                {
                    string blobName = blobItem.Name;

                    // Skip any folder placeholders (Azure may include directory markers)
                    if (blobName.EndsWith("/"))
                        continue;

                    // --- Generate SAS Token ---
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);

                    BlobSasBuilder sasBuilder = new BlobSasBuilder
                    {
                        BlobContainerName = container,
                        BlobName = blobName,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTimeOffset.UtcNow.AddDays(365)
                    };

                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    sasBuilder.ContentDisposition = "inline";
                    sasBuilder.ContentType = getContentType(blobName);

                    string sasToken = sasBuilder.ToSasQueryParameters(
                        new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                    ).ToString();

                    UriBuilder sasUriBuilder = new UriBuilder(blobClient.Uri)
                    {
                        Query = sasToken
                    };

                    string fileUrl = sasUriBuilder.Uri.ToString();
                    string fileName = Path.GetFileName(blobName);

                    result.Add((fileName, fileUrl));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to list blob URLs by folder: " + ex.Message, ex);
            }
        }

        public async Task<List<(string FileName, byte[] FileBytes)>> GetBlobFilesBySubfolder(string subFolder, string rootFolder)
        {
            var result = new List<(string FileName, byte[] FileBytes)>();

            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(rootFolder))
                    throw new ArgumentException("Root folder cannot be null or empty.");
                if (string.IsNullOrEmpty(subFolder))
                    throw new ArgumentException("Subfolder cannot be null or empty.");

                // --- Setup Azure Blob Client ---
                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                BlobServiceClient blobServiceClient = new BlobServiceClient(
                    new Uri(endpoint),
                    new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                );

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

                // --- Construct folder path prefix ---
                string prefix = $"{rootFolder.TrimEnd('/')}/{subFolder.TrimEnd('/')}/";

                // --- Get all blobs under that path ---
                var blobs = containerClient.GetBlobs(prefix: prefix);

                foreach (var blobItem in blobs)
                {
                    string blobName = blobItem.Name;

                    // Skip folder placeholders (Azure adds them sometimes)
                    if (blobName.EndsWith("/"))
                        continue;

                    // Get blob client
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);

                    // Download the blob into memory
                    using (var ms = new MemoryStream())
                    {
                        await blobClient.DownloadToAsync(ms);
                        result.Add((Path.GetFileName(blobName), ms.ToArray()));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve files for subfolder '{subFolder}': " + ex.Message, ex);
            }
        }


        #endregion

        #region date
        

        public async Task<Dictionary<string, List<(string FileName, string FileUrl)>>> GetAllBlobFilesGroupedByDate(string folderName = null)
        {
            var groupedResult = new Dictionary<string, List<(string FileName, string FileUrl)>>();

            try
            {
                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                BlobServiceClient blobServiceClient = new BlobServiceClient(
                    new Uri(endpoint),
                    new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                );

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

                string prefix = null;
                if (!string.IsNullOrEmpty(folderName))
                {
                    if (!folderName.EndsWith("/"))
                        folderName += "/";
                    prefix = folderName;
                }

                var blobs = containerClient.GetBlobs(prefix: prefix);

                foreach (var blobItem in blobs)
                {
                    string blobName = blobItem.Name;

                    if (string.IsNullOrEmpty(folderName) && blobName.Contains("/"))
                        continue;

                    if (blobName.EndsWith("/"))
                        continue;

                    BlobClient blobClient = containerClient.GetBlobClient(blobName);

                    // Generate SAS token
                    BlobSasBuilder sasBuilder = new BlobSasBuilder
                    {
                        BlobContainerName = container,
                        BlobName = blobName,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTimeOffset.UtcNow.AddDays(365)
                    };

                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    sasBuilder.ContentDisposition = "inline";
                    sasBuilder.ContentType = getContentType(blobName);

                    string sasToken = sasBuilder.ToSasQueryParameters(
                        new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                    ).ToString();

                    UriBuilder sasUriBuilder = new UriBuilder(blobClient.Uri)
                    {
                        Query = sasToken
                    };

                    string fileUrl = sasUriBuilder.Uri.ToString();
                    string fileName = Path.GetFileName(blobName);

                    DateTimeOffset? ModifiedDate = blobItem.Properties.LastModified;
                    string dateKey = ModifiedDate?.ToString("yyyyMMdd") ?? "Unknown";

                    if (!groupedResult.ContainsKey(dateKey))
                        groupedResult[dateKey] = new List<(string FileName, string FileUrl)>();

                    groupedResult[dateKey].Add((fileName, fileUrl));
                }

                groupedResult = groupedResult
                    .OrderByDescending(k => DateTime.ParseExact(k.Key, "yyyyMMdd", null))
                    .ToDictionary(k => k.Key, v => v.Value);

                return groupedResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to group blob URLs: " + ex.Message, ex);
            }
        }

        public async Task<List<(string FileName, string FileUrl)>> GetAllBlobFileUrlsByDate(string targetDate, string folderName = null)
        {
            var result = new List<(string FileName, string FileUrl)>();

            try
            {
                if (!DateTime.TryParseExact(targetDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture,
                                            System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                {
                    throw new ArgumentException("Invalid date format. Please use yyyyMMdd.");
                }

                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                BlobServiceClient blobServiceClient = new BlobServiceClient(
                    new Uri(endpoint),
                    new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                );

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

                string prefix = null;
                if (!string.IsNullOrEmpty(folderName))
                {
                    if (!folderName.EndsWith("/"))
                        folderName += "/";
                    prefix = folderName;
                }

                var blobs = containerClient.GetBlobs(prefix: prefix);

                foreach (var blobItem in blobs)
                {
                    string blobName = blobItem.Name;

                    if (string.IsNullOrEmpty(folderName) && blobName.Contains("/"))
                        continue;

                    if (blobName.EndsWith("/"))
                        continue;

                    DateTimeOffset? modified = blobItem.Properties.LastModified;
                    if (!modified.HasValue)
                        continue;

                    DateTime modifiedDate = modified.Value.Date;
                    if (modifiedDate != parsedDate.Date)
                        continue;

                    // --- Generate SAS Token ---
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);

                    BlobSasBuilder sasBuilder = new BlobSasBuilder
                    {
                        BlobContainerName = container,
                        BlobName = blobName,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTimeOffset.UtcNow.AddDays(365)
                    };

                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    sasBuilder.ContentDisposition = "inline";
                    sasBuilder.ContentType = getContentType(blobName);

                    string sasToken = sasBuilder.ToSasQueryParameters(
                        new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                    ).ToString();

                    UriBuilder sasUriBuilder = new UriBuilder(blobClient.Uri)
                    {
                        Query = sasToken
                    };

                    string fileUrl = sasUriBuilder.Uri.ToString();
                    string fileName = Path.GetFileName(blobName);

                    result.Add((fileName, fileUrl));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to list blob URLs by date: " + ex.Message, ex);
            }
        }

        public async Task<List<(string FileName, byte[] FileBytes)>> GetBlobFileByDate(string dateString, string folderName = null)
        {
            var result = new List<(string FileName, byte[] FileBytes)>();

            try
            {
                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                BlobServiceClient blobServiceClient = new BlobServiceClient(
                    new Uri(endpoint),
                    new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                );

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

                string prefix = null;
                if (!string.IsNullOrEmpty(folderName))
                {
                    if (!folderName.EndsWith("/"))
                        folderName += "/";
                    prefix = folderName;
                }

                DateTime targetDate = DateTime.ParseExact(dateString, "yyyyMMdd", null);

                var blobs = containerClient.GetBlobs(prefix: prefix);

                foreach (var blobItem in blobs)
                {
                    string blobName = blobItem.Name;

                    if (string.IsNullOrEmpty(folderName) && blobName.Contains("/"))
                        continue;

                    if (blobName.EndsWith("/"))
                        continue;

                    DateTimeOffset? modified = blobItem.Properties.LastModified;
                    if (modified.HasValue && modified.Value.Date == targetDate.Date)
                    {
                        BlobClient blobClient = containerClient.GetBlobClient(blobName);
                        using (var ms = new MemoryStream())
                        {
                            await blobClient.DownloadToAsync(ms);
                            result.Add((Path.GetFileName(blobName), ms.ToArray()));
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve files for date " + dateString + ": " + ex.Message, ex);
            }
        }


        #endregion

        #region COA - group by date extract from file name

        public async Task<Dictionary<string, List<(string FileName, string FileUrl)>>> GetCOAFilesGroupedByDate(string folderName = null)
        {
            var groupedResult = new Dictionary<string, List<(string FileName, string FileUrl)>>();

            try
            {
                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                BlobServiceClient blobServiceClient = new BlobServiceClient(
                    new Uri(endpoint),
                    new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                );

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

                string prefix = null;
                if (!string.IsNullOrEmpty(folderName))
                {
                    if (!folderName.EndsWith("/"))
                        folderName += "/";
                    prefix = folderName;
                }

                var blobs = containerClient.GetBlobs(prefix: prefix);

                foreach (var blobItem in blobs)
                {
                    string blobName = blobItem.Name;

                    // Skip subfolders or directories
                    if (string.IsNullOrEmpty(folderName) && blobName.Contains("/"))
                        continue;

                    if (blobName.EndsWith("/"))
                        continue;

                    BlobClient blobClient = containerClient.GetBlobClient(blobName);

                    // Generate SAS token
                    BlobSasBuilder sasBuilder = new BlobSasBuilder
                    {
                        BlobContainerName = container,
                        BlobName = blobName,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTimeOffset.UtcNow.AddDays(365)
                    };

                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    sasBuilder.ContentDisposition = "inline";
                    sasBuilder.ContentType = getContentType(blobName);

                    string sasToken = sasBuilder.ToSasQueryParameters(
                        new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                    ).ToString();

                    UriBuilder sasUriBuilder = new UriBuilder(blobClient.Uri)
                    {
                        Query = sasToken
                    };

                    string fileUrl = sasUriBuilder.Uri.ToString();
                    string fileName = Path.GetFileName(blobName);

                    // Extract date from filename (e.g. COA_01142231204_2025-10-31_114547.pdf)
                    string dateKey = "Unknown";
                    try
                    {
                        string[] parts = fileName.Split('_');
                        if (parts.Length >= 3)
                        {
                            // Example: parts[2] = "2025-10-31"
                            string datePart = parts[2];
                            if (DateTime.TryParse(datePart, out DateTime parsedDate))
                            {
                                dateKey = parsedDate.ToString("yyyyMMdd");
                            }
                        }
                    }
                    catch
                    {
                        dateKey = "Unknown";
                    }

                    if (!groupedResult.ContainsKey(dateKey))
                        groupedResult[dateKey] = new List<(string FileName, string FileUrl)>();

                    groupedResult[dateKey].Add((fileName, fileUrl));
                }

                // Sort by date descending (ignore "Unknown")
                groupedResult = groupedResult
                    .Where(k => k.Key != "Unknown")
                    .OrderByDescending(k => DateTime.ParseExact(k.Key, "yyyyMMdd", null))
                    .Concat(groupedResult.Where(k => k.Key == "Unknown")) // Add Unknown last
                    .ToDictionary(k => k.Key, v => v.Value);

                return groupedResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to group blob URLs by file date: " + ex.Message, ex);
            }
        }

        public async Task<List<(string FileName, string FileUrl)>> GetCOAFileUrlsByDate(string targetDate, string folderName = null)
        {
            var result = new List<(string FileName, string FileUrl)>();

            try
            {
                // Validate targetDate format (yyyyMMdd)
                if (!DateTime.TryParseExact(targetDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture,
                                            System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                {
                    throw new ArgumentException("Invalid date format. Please use yyyyMMdd.");
                }

                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                BlobServiceClient blobServiceClient = new BlobServiceClient(
                    new Uri(endpoint),
                    new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                );

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

                string prefix = null;
                if (!string.IsNullOrEmpty(folderName))
                {
                    if (!folderName.EndsWith("/"))
                        folderName += "/";
                    prefix = folderName;
                }

                var blobs = containerClient.GetBlobs(prefix: prefix);

                foreach (var blobItem in blobs)
                {
                    string blobName = blobItem.Name;

                    // Skip subfolders or directories
                    if (string.IsNullOrEmpty(folderName) && blobName.Contains("/"))
                        continue;
                    if (blobName.EndsWith("/"))
                        continue;

                    string fileName = Path.GetFileName(blobName);

                    // Extract date from file name (format: COA_01142231204_2025-10-31_114547.pdf)
                    string dateKey = null;
                    try
                    {
                        string[] parts = fileName.Split('_');
                        if (parts.Length >= 3)
                        {
                            string datePart = parts[2]; // e.g. "2025-10-31"
                            if (DateTime.TryParseExact(datePart, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                                                       System.Globalization.DateTimeStyles.None, out DateTime dateFromFile))
                            {
                                dateKey = dateFromFile.ToString("yyyyMMdd");
                            }
                        }
                    }
                    catch
                    {
                        // Ignore invalid file names
                    }

                    // Skip files without valid date or not matching the target date
                    if (string.IsNullOrEmpty(dateKey) || dateKey != targetDate)
                        continue;

                    // --- Generate SAS Token ---
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);

                    BlobSasBuilder sasBuilder = new BlobSasBuilder
                    {
                        BlobContainerName = container,
                        BlobName = blobName,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTimeOffset.UtcNow.AddDays(365)
                    };

                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    sasBuilder.ContentDisposition = "inline";
                    sasBuilder.ContentType = getContentType(blobName);

                    string sasToken = sasBuilder.ToSasQueryParameters(
                        new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                    ).ToString();

                    UriBuilder sasUriBuilder = new UriBuilder(blobClient.Uri)
                    {
                        Query = sasToken
                    };

                    string fileUrl = sasUriBuilder.Uri.ToString();

                    result.Add((fileName, fileUrl));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to list blob URLs by file date: " + ex.Message, ex);
            }
        }

        public async Task<List<(string FileName, byte[] FileBytes)>> GetCOABlobFileByDate(string dateString, string folderName = null)
        {
            var result = new List<(string FileName, byte[] FileBytes)>();

            try
            {
                // Validate input date format (yyyyMMdd)
                if (!DateTime.TryParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture,
                                            System.Globalization.DateTimeStyles.None, out DateTime targetDate))
                {
                    throw new ArgumentException("Invalid date format. Please use yyyyMMdd.");
                }

                string endpoint = AzureBlobString.blobServiceEndpoint;
                string container = common.getContainerName();

                BlobServiceClient blobServiceClient = new BlobServiceClient(
                    new Uri(endpoint),
                    new StorageSharedKeyCredential(AzureBlobString.StorageAccountName, AzureBlobString.key)
                );

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

                string prefix = null;
                if (!string.IsNullOrEmpty(folderName))
                {
                    if (!folderName.EndsWith("/"))
                        folderName += "/";
                    prefix = folderName;
                }

                var blobs = containerClient.GetBlobs(prefix: prefix);

                foreach (var blobItem in blobs)
                {
                    string blobName = blobItem.Name;

                    // Skip folders or nested paths
                    if (string.IsNullOrEmpty(folderName) && blobName.Contains("/"))
                        continue;
                    if (blobName.EndsWith("/"))
                        continue;

                    string fileName = Path.GetFileName(blobName);

                    // Extract date from file name: e.g. COA_01142231204_2025-10-31_114547.pdf
                    string extractedDate = null;
                    try
                    {
                        string[] parts = fileName.Split('_');
                        if (parts.Length >= 3)
                        {
                            string datePart = parts[2]; // "2025-10-31"
                            if (DateTime.TryParseExact(datePart, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                                                       System.Globalization.DateTimeStyles.None, out DateTime fileDate))
                            {
                                extractedDate = fileDate.ToString("yyyyMMdd");
                            }
                        }
                    }
                    catch
                    {
                        // Ignore files with unexpected names
                    }

                    // Skip files that don't match the target date
                    if (string.IsNullOrEmpty(extractedDate) || extractedDate != dateString)
                        continue;

                    // --- Download matching file ---
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);
                    using (var ms = new MemoryStream())
                    {
                        await blobClient.DownloadToAsync(ms);
                        result.Add((fileName, ms.ToArray()));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve files for date {dateString}: {ex.Message}", ex);
            }
        }

        #endregion
    }
}