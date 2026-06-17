using System.Collections.Generic;
using Titanic.CDN.Models;

namespace Titanic.CDN.Requests
{
    public class PresignUploadRequest : CDNRequest<AdminPresignUploadResponseModel>
    {
        public string ObjectKey { get; set; }
        public string? ContentType { get; set; }

        public PresignUploadRequest(string objectKey, string? contentType = null)
        {
            TitanicCDN.EscapeObjectKey(objectKey);
            ObjectKey = objectKey;
            ContentType = contentType;
        }

        protected override AdminPresignUploadResponseModel Execute(TitanicCDN cdn)
        {
            cdn.EnsureAccessKey();

            Dictionary<string, string>? headers = null;
            if (!string.IsNullOrEmpty(ContentType))
                headers = new Dictionary<string, string> { ["Content-Type"] = ContentType! };

            return cdn.Post<AdminPresignUploadResponseModel>($"/admin/files/{TitanicCDN.EscapeObjectKey(ObjectKey)}", headers);
        }
    }
}
