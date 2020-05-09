using Microsoft.AspNetCore.Http;
using System.IO;

namespace ComputerVisionWeb.Helper
{
    public static class HelperClass
    {
        public static byte[] ReadFully(IFormFile File)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                File.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
