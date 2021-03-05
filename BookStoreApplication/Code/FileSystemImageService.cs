using BookStoreApi.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace BookStoreApi.Code {
    public class FileSystemImageService : IImageService {

        private readonly IConfiguration _Configuration;
        private readonly IWebHostEnvironment _Environement;

        public FileSystemImageService(IConfiguration configuration,
            IWebHostEnvironment environement) {
            _Configuration = configuration;
            _Environement = environement;
        }

        private Size ThumbnailSize {
            get {
                int height = _Configuration.GetValue<int>("Images:ThumbHeight", 96);
                int width = _Configuration.GetValue<int>("Images:ThumbWidth", 96);
                return new Size(width, height);
            }

        }

        private ImageFormat ImageFormat {
            get {
                string thumbnailMimeType = _Configuration.GetValue<string>("Images:ThumbMimeRype", "image/png");
                return GetImageFormat(thumbnailMimeType);
            }
        }

        private static ImageFormat GetImageFormat(string mimeType) {
            return mimeType switch {
                MediaTypeNames.Image.Jpeg => ImageFormat.Jpeg,
                "image/bmp" => ImageFormat.Bmp,
                MediaTypeNames.Image.Gif => ImageFormat.Gif,
                "image/png" => ImageFormat.Png,
                _ => throw new ArgumentOutOfRangeException($"Invalid MimeType {mimeType} for thumbnail"),
            };
        }


        public static string GetMimeType(ImageFormat imageFormat) {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
        }

        public static string GetExtension(ImageFormat imageFormat) {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.FormatID == imageFormat.Guid).FilenameExtension;
        }

        private string GetLocation() {
            string defaultLocation = Path.Combine(_Environement.ContentRootPath, "Uploads");
            return _Configuration.GetValue<string>("Images:Location", defaultLocation);
        }

        private string GenerateFileName(string extension = "") {
            string location = GetLocation();
            string fileName = string.Empty;
            bool fileExists = true;
            while (fileExists) {
                fileName = Path.GetRandomFileName();
                if (!String.IsNullOrWhiteSpace(extension))
                    fileExists = File.Exists(Path.Combine(location, fileName + "." + extension));
                else
                    fileExists = Directory.GetFiles(location, fileName + ".*").Length > 0;
            }
            return fileName;
        }

        private string GetImageFile(string identifier) {
            var possibleFiles = Directory.GetFiles(GetLocation(), identifier);
            return possibleFiles?.FirstOrDefault();
        }
        public async Task<string> CreateThumbnail(string base64Content) {
            using Stream imageStream = new MemoryStream(await ImageTools.GetBytesFromBase64String(base64Content));
            return await CreateThumbnail(imageStream);

        }

        public async Task<string> CreateThumbnail(Stream imageStream) {
            using Stream thumbnailStream = await ImageTools.GetImageThumbnail(imageStream, ThumbnailSize.Height, ThumbnailSize.Width, ImageFormat);
            string mimeType = GetMimeType(ImageFormat);
            string thumbnaimString = await ImageTools.GetBase64StringAsync(thumbnailStream);
            return String.Format("data:{0};base64,{1}", mimeType, thumbnaimString);
        }


        /// <summary>
        /// This method can decrease image quality, so call it only when image is 
        /// </summary>
        /// <param name="base64Content"></param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public Task<ImageData> SetImage(string base64Content, CancellationToken cancellationToken, string identifier = "") {
            return Task.Run(async () => {
                cancellationToken.ThrowIfCancellationRequested();
                var imageBytes = ImageTools.GetBytesFromBase64String(base64Content);
                Task<ImageFormat> mediaTypeTask = Task.Run(async () => {
                    using Stream bitmapStream = new MemoryStream(await imageBytes);
                    using Bitmap bitmap = new Bitmap(bitmapStream);
                    return bitmap.RawFormat;
                });

                cancellationToken.ThrowIfCancellationRequested();
                Task<string> identifierTask = Task.Run(async () => {
                    if (String.IsNullOrWhiteSpace(identifier))
                        return GenerateFileName(GetExtension(await mediaTypeTask));
                    else {
                        await RemoveImage(identifier);
                        return identifier;
                    }
                });

                cancellationToken.ThrowIfCancellationRequested();
                Task<string> fileNameTask = Task.Run(async () => {
                    string fileName = Path.Combine(GetLocation(), await identifierTask);
                    return fileName;
                });

                cancellationToken.ThrowIfCancellationRequested();
                Task writeFileTask = Task.Run(async () => {
                    using var imageBytesStream = new MemoryStream(await imageBytes);
                    using FileStream fileStream = new FileStream(await fileNameTask, FileMode.Create);
                    await imageBytesStream.CopyToAsync(fileStream);
                });

                cancellationToken.ThrowIfCancellationRequested();
                var thumbnailTask = CreateThumbnail(new MemoryStream(await imageBytes));
                await writeFileTask;

                if (cancellationToken.IsCancellationRequested) {
                    try {
                        await RemoveImage(await identifierTask);
                    }
                    finally {

                    }
                }
                return new ImageData() {
                    Base64ThumbNail = await thumbnailTask,
                    Changed = true,
                    Base64Image = base64Content,
                    Name = await identifierTask,
                    MediaType = GetMimeType(await mediaTypeTask)
                };
            });
        }

        public Task<ImageData> GetImage(string identifier) {
            return Task.Run(async () => {
                ImageData imageData = new ImageData() { Name = identifier };
                string imageFile = GetImageFile(identifier);
                if (string.IsNullOrWhiteSpace(imageFile))
                    throw new FileNotFoundException($"File {imageFile} was not found in uploads directory({GetLocation()})");
                using FileStream fileStream = new FileStream(imageFile, FileMode.Open);
                using Bitmap bitmap = new Bitmap(fileStream);
                imageData.MediaType = GetMimeType(bitmap.RawFormat);
                if (fileStream.CanSeek) {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    imageData.Base64Image = await ImageTools.GetBase64StringAsync(fileStream);
                }
                return imageData;
            });
        }

        public Task RemoveImage(string identifier) {
            var fileName = GetImageFile(identifier);
            if (!String.IsNullOrWhiteSpace(fileName))
                File.Delete(fileName);
            return Task.CompletedTask;
        }

    }
}
