using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.WASM.Data.Models {
    public class RepositoryResponce {
        public bool Succeed { get; set; }

        public int StatusCode { get; set; }

        public string Errror { get; set; }

        public string Message { get; set; }

        public RepositoryResponce() {
            ;
        }

        public static RepositoryResponce SucceeedResponce => new RepositoryResponce() {
            Succeed = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Operation completes succesfully"
        };

        public static RepositoryResponce ArgumentNullResponce => new RepositoryResponce() {
            Succeed = false,
            Message = "Null argument is not allowed for this case",
            StatusCode = StatusCodes.Status400BadRequest,
            Errror = "Argument you passed to method in null"
        };

        public static RepositoryResponce StatusCodeResponce(System.Net.HttpStatusCode statusCode) {
            return new RepositoryResponce() {
                StatusCode = (int)statusCode,
                Message = statusCode.ToString(),
                Succeed = ((int)statusCode >= 200) && ((int)statusCode <= 299)
            };
        }

    }
}
