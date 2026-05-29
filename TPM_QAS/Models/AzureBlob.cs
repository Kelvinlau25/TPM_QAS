using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TPM_QAS.Models
{
    public class AzureBlob
    {
        public static class AzureBlobString
        {
            public static string connString = "DefaultEndpointsProtocol=https;AccountName=tpmrg01diag;AccountKey=ZqYQItCsFwR7exrT9BbAl36I6wnauCSeADIcdBbuG8Oa4CTYbsJPsAFB43Bz6I+txIc3RGIBJX1Fsr9hSmrEKA==;EndpointSuffix=core.windows.net";
            public static string key = "ZqYQItCsFwR7exrT9BbAl36I6wnauCSeADIcdBbuG8Oa4CTYbsJPsAFB43Bz6I+txIc3RGIBJX1Fsr9hSmrEKA==";
            //public static string CredentialID = "szekhai.foo.b6@mail.toray";
            //public static string CredentialPass = "HcXz8246";
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
            public List<HttpPostedFileBase> PostedFile { get; set; }
            public AzureBlobModel()
            {
                PostedFile = new List<HttpPostedFileBase>();
            }
        }

        public class CustomHttpPostedFileBase : HttpPostedFileBase
        {
            private readonly Stream _stream;
            private readonly string _fileName;
            private readonly string _contentType;

            public CustomHttpPostedFileBase(Stream stream, string fileName, string contentType)
            {
                _stream = stream;
                _fileName = fileName;
                _contentType = contentType;
            }

            public override int ContentLength => (int)_stream.Length;

            public override string ContentType => _contentType;

            public override string FileName => _fileName;

            public override Stream InputStream => _stream;
        }
    }
}