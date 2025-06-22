using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace florist.PageAddEdit
{
    public class ImageHelper
    {
        public static string GetProjectImagesPath()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string projectPath = Path.GetFullPath(Path.Combine(appPath, @"..\..\"));
            string imagesPath = Path.Combine(projectPath, "Images");
            return imagesPath;
        }

        public static string CopyImageToProject(string sourcePath)
        {
            try
            {
                string fileName = Path.GetFileName(sourcePath);
                string imagesFolder = GetProjectImagesPath();

                // Проверяем существует ли папка Images
                if (!Directory.Exists(imagesFolder))
                {
                    throw new DirectoryNotFoundException($"Папка с изображениями не найдена: {imagesFolder}");
                }

                string destPath = Path.Combine(imagesFolder, fileName);

                // Если файл уже существует, просто используем его
                if (File.Exists(destPath))
                {
                    return fileName;
                }

                // Копируем только если файла еще нет
                File.Copy(sourcePath, destPath);
                return fileName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка работы с изображением: {ex.Message}");
            }
        }

        public static Uri GetImageUri(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                return null;

            string imagePath = Path.Combine(GetProjectImagesPath(), imageName);
            if (File.Exists(imagePath))
                return new Uri(imagePath, UriKind.Absolute);

            return null;
        }
    }
}
