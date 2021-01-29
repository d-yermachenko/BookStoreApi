using AutoMapper;
using BookStoreApi.Data;
using BookStoreApi.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Code.AutoMapper {
    public class AutoMapperConfig : Profile {
        public AutoMapperConfig() {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Author, AuthorUpsertDTO>().ReverseMap().ForMember(o => o.Id, a => a.Ignore());
            CreateMap<Book, BookDTO>().ReverseMap();

        }
    }
}
