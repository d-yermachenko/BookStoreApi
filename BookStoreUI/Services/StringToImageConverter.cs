using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUI.Services {
    public static class StringToImageConverter {
        public static async Task<string> GetBase64StringAsync(this Stream inStream) {
            return await Task.Run( async () => {
                using MemoryStream memoryStream = new MemoryStream();
                await inStream.CopyToAsync(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            });
            
        }

    }
}
