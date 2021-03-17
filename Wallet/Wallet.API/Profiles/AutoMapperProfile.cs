using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.API.Models;
using Wallet.Data.Models;

namespace Wallet.API.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Agregar mapeos necesarios acá:
            //CreateMap<Example, ExampleViewModel>();
            CreateMap<Users, RegisterModel>().ReverseMap();
            CreateMap<Users, LoginModel>().ReverseMap();
            CreateMap<Transactions, TransactionModel>().ReverseMap();
        }
    }
}
