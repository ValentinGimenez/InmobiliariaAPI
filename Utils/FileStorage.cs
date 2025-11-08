using Microsoft.AspNetCore.Hosting;

namespace _net_integrador.Utils
{
    public static class FileStorage
    {
        private static readonly string[] Allowed = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public static string SaveImage(IFormFile file, IWebHostEnvironment env, string subfolder = "Uploads")
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!Allowed.Contains(ext)) throw new InvalidOperationException("Formato de imagen no permitido.");

            var root = env.WebRootPath ?? "wwwroot";
            var folder = Path.Combine(root, subfolder);
            Directory.CreateDirectory(folder);

            var name = $"{Guid.NewGuid()}{ext}";
            var full = Path.Combine(folder, name);

            using (var fs = new FileStream(full, FileMode.Create))
                file.CopyTo(fs);

            return Path.Combine(subfolder, name).Replace("\\", "/");
        }

        public static void DeleteFile(string relativePath, IWebHostEnvironment env)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;
            var root = env.WebRootPath ?? "wwwroot";
            var full = Path.Combine(root, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (System.IO.File.Exists(full))
                System.IO.File.Delete(full);
        }
    }
}
