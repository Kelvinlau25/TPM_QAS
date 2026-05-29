using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace TPM_QAS.Models
{
    public class AzureBlob
    {
        public static class AzureBlobString
        {
            public static string connString = "DefaultEndpointsProtocol=https;AccountName=tpmrg01diag;AccountKey=ZqYQItCsFwR7exrT9BbAl36I6wnauCSeADIcdBbuG8Oa4CTYbsJPsAFB43Bz6I+txIc3RGIBJX1Fsr9hSmrEKA==;EndpointSuffix=core.windows.net";
            public static string key = "ZqYQItCsFwR7exrT9BbAl36I6wnauCSeADIcdBbuG8Oa4CTYbsJPsAFB43Bz6I+txIc3RGIBJX1Fsr9hSmrEKA==";
            public static string StorageAccountName = "tpmrg01diag";
            public static string blobServiceEndpoint = "https://tpmrg01diag.blob.core.windows.net/";
        }

        public class AzureBlobModel
        {
            public string AZURE_ACC_NAME { get; set; }
            public string AZURE_KEY { get; set; }
            public string AZURE_CON_STR { get; set; }
            public string CREDENTIAL_ID { get; set; }
            public string CREDENTIAL_PASS { get; set; }
            public string END_POINT { get; set; }
            public string CONTAINER { get; set; }
            public string DIRECTORY { get; set; }
            public List<IFormFile> PostedFile { get; set; }
            public AzureBlobModel()
            {
                PostedFile = new List<IFormFile>();
            }
        }

        public class CustomFormFile : IFormFile
        {
            private readonly Stream _stream;
            private readonly string _fileName;
            private readonly string _contentType;

            public CustomFormFile(Stream stream, string fileName, string contentType)
            {
                _stream = stream;
                _fileName = fileName;
                _contentType = contentType;
            }

            public string ContentType => _contentType;
            public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{_fileName}\"";
            public IHeaderDictionary Headers => new HeaderDictionary();
            public long Length => _stream.Length;
            public string Name => "file";
            public string FileName => _fileName;

            public void CopyTo(Stream target) => _stream.CopyTo(target);
            public async System.Threading.Tasks.Task CopyToAsync(Stream target, System.Threading.CancellationToken cancellationToken = default) => await _stream.CopyToAsync(target, cancellationToken);
            public Stream OpenReadStream() => _stream;
        }
    }
}
