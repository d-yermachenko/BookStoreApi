using BookStoreUI.Data.DTOs;
using BookStoreUI.Data.Models;
using BookStoreUI.Data.ViewModels.Authentification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Contracts {
    interface IAuthentificationRepository {
        Task<RepositoryResponce> Register(RegistrationVM userData);

        Task<RepositoryResponce> Login(UserLoginDTO loginVM);

        Task Logout();
    }
}
