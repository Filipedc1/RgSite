using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace RgSite.Core.Helpers
{
    public static class ImageUtil
    {
        public static bool UploadImage(IFormFile image, string webRootPath)
        {
            try
            {
                string filePath = Path.Combine($"{webRootPath}\\images", Path.GetFileName(image.FileName));

                using var fs = new FileStream(filePath, FileMode.Create);
                if (!File.Exists(filePath))
                    image.CopyTo(fs);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool DeleteImage(string imagePath, string webRootPath)
        {
            try
            {
                string filePath = Path.Combine($"{webRootPath}\\images", Path.GetFileName(imagePath));

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
