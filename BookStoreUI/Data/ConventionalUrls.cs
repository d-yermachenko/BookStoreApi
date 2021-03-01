using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Data {
    public static class ConventionalUrls {
        public const string BaseUrlConfigurationKey = "ConnectionStrings:DefaultUrl";

        public const string RegisterRelativeUrl = "/api/Users/register";

        public const string LoginRelativeUrl = "/api/Users/login";

        public const string Authors = "/api/Authors/";

        public const string Books = "/api/Books/";

        public const string TokenStorageKey = "";
    }
}
