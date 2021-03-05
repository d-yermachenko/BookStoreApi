using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Code {
    public static class ImageTools {
        public static async Task<string> GetBase64StringAsync(Stream inStream) {
            return await Task.Run(async () => {
                using MemoryStream memoryStream = new MemoryStream();
                await inStream.CopyToAsync(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            });

        }

        public static async Task<byte[]> GetBytesFromBase64String(string data) {
            return await Task.Run(() => {
                return Convert.FromBase64String(data);

            });
        }


        public static async Task<Stream> GetImageThumbnail(Stream source, int height, int width, ImageFormat format = null ) {
            return await Task.Run(() => {
                Bitmap bitmap = new Bitmap(source);
                if (format == null)
                    format = bitmap.RawFormat;
                var thumbnail = bitmap.GetThumbnailImage(width, height, null, IntPtr.Zero);
                MemoryStream saveStream = new MemoryStream();
                thumbnail.Save(saveStream, format);
                saveStream.Seek(0, SeekOrigin.Begin);
                return saveStream as Stream;
            });
        }
    }
}
