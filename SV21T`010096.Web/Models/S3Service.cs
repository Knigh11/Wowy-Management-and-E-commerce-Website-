using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace SV21T1020096.Web.Models
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string? _bucketName;

        public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            if (configuration["AWS:BucketName"] != null)
            {
                _bucketName = configuration["AWS:BucketName"];
            }
            else
            {
                _bucketName = null;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            var fileName = $"{folderName}/{Guid.NewGuid()}-{file.FileName}";

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = ms,
                Key = fileName,
                BucketName = _bucketName,
                ContentType = file.ContentType
            };

            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);

            return fileName;
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{fileName}"
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

        public string GetFileUrl(string fileName)
        {
            return $"https://litecommercdb.s3.ap-southeast-2.amazonaws.com/{fileName}";
        }
    }
}
