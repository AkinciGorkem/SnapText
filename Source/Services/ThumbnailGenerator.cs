using System.Drawing;
using System.Drawing.Imaging;

namespace SnapText.Services
{
    public static class ThumbnailGenerator
    {
        private const int ThumbnailSize = 150;

        public static string GenerateThumbnail(string originalImagePath)
        {
            try
            {
                var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnapText", "Thumbnails");
                Directory.CreateDirectory(appDataPath);

                var fileName = Path.GetFileNameWithoutExtension(originalImagePath) + "_thumb.png";
                var thumbnailPath = Path.Combine(appDataPath, fileName);

                using (var originalImage = Image.FromFile(originalImagePath))
                {
                    var thumbnail = CreateThumbnail(originalImage, ThumbnailSize, ThumbnailSize);
                    thumbnail.Save(thumbnailPath, ImageFormat.Png);
                    thumbnail.Dispose();
                }

                return thumbnailPath;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GenerateThumbnail(Image originalImage, string baseFileName)
        {
            try
            {
                var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnapText", "Thumbnails");
                Directory.CreateDirectory(appDataPath);

                var fileName = baseFileName + "_thumb.png";
                var thumbnailPath = Path.Combine(appDataPath, fileName);

                var thumbnail = CreateThumbnail(originalImage, ThumbnailSize, ThumbnailSize);
                thumbnail.Save(thumbnailPath, ImageFormat.Png);
                thumbnail.Dispose();

                return thumbnailPath;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static Image CreateThumbnail(Image originalImage, int maxWidth, int maxHeight)
        {
            var originalWidth = originalImage.Width;
            var originalHeight = originalImage.Height;

            var ratioX = (double)maxWidth / originalWidth;
            var ratioY = (double)maxHeight / originalHeight;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(originalWidth * ratio);
            var newHeight = (int)(originalHeight * ratio);

            var thumbnail = new Bitmap(maxWidth, maxHeight);
            
            using (var graphics = Graphics.FromImage(thumbnail))
            {
                graphics.Clear(Color.White);
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                var x = (maxWidth - newWidth) / 2;
                var y = (maxHeight - newHeight) / 2;
                
                graphics.DrawImage(originalImage, x, y, newWidth, newHeight);
            }

            return thumbnail;
        }
    }
}