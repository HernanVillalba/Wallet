using AutoMapper;
using Wallet.Data.Models;
using Wallet.Entities;

namespace Wallet.Business.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Users, RegisterModel>().ReverseMap();
            CreateMap<Users, LoginModel>().ReverseMap();
            CreateMap<Transactions, TransactionCreateModel>().ReverseMap();
            CreateMap<FixedTermDeposit, FixedTermDepositModel>().ReverseMap();
            CreateMap<Transactions, TransactionFilterModel>().ReverseMap();
        }
    }
}
