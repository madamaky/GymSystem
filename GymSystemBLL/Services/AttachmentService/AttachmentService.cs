using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GymSystemBLL.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private readonly long maxFileSize = 5 * 1024 * 1024; // 5 MB

        public string? Upload(string folderName, IFormFile file)
        {
            try
            {
                if (folderName is null || file is null || file.Length == 0) return null;

                if (file.Length > maxFileSize) return null;

                var ext = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(ext)) return null;

                var FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", folderName);
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }

                var FileName = Guid.NewGuid().ToString() + ext;

                var FilePath = Path.Combine(FolderPath, FileName);

                using var FileStream = new FileStream(FilePath, FileMode.Create);

                file.CopyTo(FileStream);

                return FileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed To Upload Photo : {ex}");
                return null;
            }
        }

        public bool Delete(string fileName, string folderName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return false;

                var FullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", folderName, fileName);
                if (File.Exists(FullPath))
                {
                    File.Delete(FullPath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed To Delete Photo : {ex}");
                return false;
            }
        }
    }
}
