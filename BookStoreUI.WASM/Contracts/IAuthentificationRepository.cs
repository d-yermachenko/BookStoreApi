using BookStoreUI.WASM.Data.DTOs;
using BookStoreUI.WASM.Data.Models;
using BookStoreUI.WASM.Data.ViewModels.Authentification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.WASM.Contracts {
    interface IAuthentificationRepository {
        Task<RepositoryResponce> Register(RegistrationVM userData);

        Task<RepositoryResponce> Login(UserLoginDTO loginVM);

        Task Logout();
    }
}
