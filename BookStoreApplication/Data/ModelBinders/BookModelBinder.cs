using BookStoreApi.Data.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Data.ModelBinders {
    public class BookModelBinder : IModelBinder {
        public async Task BindModelAsync(ModelBindingContext bindingContext) {
            string modelContent ;
            using var bodyReader = new System.IO.StreamReader(
                bindingContext.HttpContext.Request.Body,
                System.Text.Encoding.UTF8);
            bindingContext.ModelName = bindingContext.OriginalModelName;

            modelContent = await bodyReader.ReadToEndAsync();
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<BookUpsertDTO>(modelContent);
            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}
