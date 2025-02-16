namespace SV21T1020096.Web.Models
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        Task DeleteFileAsync(string fileName);
        string GetFileUrl(string fileName);
    }
}
