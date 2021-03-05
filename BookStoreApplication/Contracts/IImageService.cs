using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookStoreApi.Contracts {

    public class ImageData {
        public bool Changed { get; set; } = false;

        public String Base64Image { get; set; } = String.Empty;

        public String Base64ThumbNail { get; set; } = String.Empty;

        public String MediaType { get; set; } = String.Empty;

        public String Name { get; set; } = String.Empty;
    }

    public interface IImageService {

        /// <summary>
        /// Saves image to configured location and returns its id to save
        /// This method can decrease image quality, so call it only when image was changed
        /// </summary>
        /// <returns></returns>
        public Task<ImageData> SetImage(string base64Content, CancellationToken cancellationToken, string identifier);

        /// <summary>
        /// Gets image as base64 string
        /// </summary>
        /// <param name="identifier">Id of image</param>
        /// <returns>Content of image as base64 string</returns>
        public Task<ImageData> GetImage(string identifier);

        public Task RemoveImage(string identifier);



    }
}
