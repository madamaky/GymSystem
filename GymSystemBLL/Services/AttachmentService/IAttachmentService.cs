using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GymSystemBLL.Services.AttachmentService
{
    public interface IAttachmentService
    {
        // Function To Upload Photo and Return Photo Name
        string? Upload(string folderName, IFormFile file);

        // Function To Delete Photo
        bool Delete(string fileName, string folderName);
    }
}
