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
            CreateMap<Transactions, TransactionModel>().ReverseMap();
            CreateMap<FixedTermDeposit, FixedTermDepositCreateModel>().ReverseMap();
            CreateMap<FixedTermDeposit, FixedTermDepositModel>().ReverseMap();
            CreateMap<Transactions, TransactionSearchModel>().ReverseMap();
        }
    }
}
