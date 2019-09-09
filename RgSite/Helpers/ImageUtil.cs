using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Helpers
{
    public static class ImageUtil
    {
        public static bool UploadImage(IFormFile img, IHostingEnvironment hosting)
        {
            try
            {
                string filePath = Path.Combine(hosting.WebRootPath + "\\images", Path.GetFileName(img.FileName));

                if (!System.IO.File.Exists(filePath))
                    img.CopyTo(new FileStream(filePath, FileMode.Create));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool DeleteImage(string imgPath, IHostingEnvironment hosting)
        {
            try
            {
                string filePath = Path.Combine(hosting.WebRootPath + "\\images", Path.GetFileName(imgPath));

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
