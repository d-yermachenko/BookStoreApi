﻿using BookStoreUI.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Contracts {
    public interface IBookRepository : IRepository<Book, int> {

    }
}
